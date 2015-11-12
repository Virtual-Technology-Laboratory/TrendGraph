/*
 * Copyright (c) 2014, Roger Lew (rogerlew.gmail.com)
 * Date: 5/20/2015
 * License: BSD (3-clause license)
 * 
 * The project described was supported by NSF award number IIA-1301792
 * from the NSF Idaho EPSCoR Program and by the National Science Foundation.
 * 
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VTL.TrendGraph
{
    public struct TimeseriesRecord
    {
        public DateTime time;
        public float value;

        public TimeseriesRecord(DateTime time, float value)
        {
            this.time = time;
            this.value = value;
        }
    }

    public class TrendGraphController : MonoBehaviour
    {
        public Color lineColor = Color.white;
        public float lineWidth = 1f;
        public float yMax = 1;
        public float yMin = 0;
        public float timebase = 300; // in seconds
        public string timebaseLabel = "-5 min"; // Its on the developer to make sure 
                                                // this makes sense with the timebase
        public string unitsLabel = "F"; // the units label
        public string valueFormatString = "D3";
        private DateTime lastDraw;
        
        List<TimeseriesRecord> timeseries;
        Text valueText;
        public int w = 256;
        public int h = 256;

        Image graphImage;
        Texture2D trendTexture = null;
        Color32[] texColors;
        public void OnValidate()
        {
            transform.Find("Ymax")
                     .GetComponent<Text>()
                     .text = yMax.ToString();

            transform.Find("Ymin")
                     .GetComponent<Text>()
                     .text = yMin.ToString();

            transform.Find("Timebase")
                     .GetComponent<Text>()
                     .text = timebaseLabel;

            transform.Find("Units")
                     .GetComponent<Text>()
                     .text = unitsLabel;
        }

        // Use this for initialization
        void Start()
        {
            timeseries = new List<TimeseriesRecord>();
            graphImage = transform.Find("Graph")
                                  .Find("GraphImage")
                                  .GetComponent<Image>();

            valueText = transform.Find("Value").GetComponent<Text>();
        }

        void Update()
        {
            // Set the width and height to integers
            int width = w;
            int height = h;

            // Sort the time series
            timeseries.Sort((s1, s2) => s1.time.CompareTo(s2.time));

            // cull old records
            if (timeseries.Count == 0) return;
            var elapsed = (float)(lastDraw - timeseries[0].time).TotalSeconds;
            while (elapsed > timebase && elapsed > 0)
            {
                timeseries.RemoveAt(0);
                if (timeseries.Count == 0) return;
                elapsed = (float)(lastDraw - timeseries[0].time).TotalSeconds;
            }

            // cull future records
            // e.g. SimTimeControl, user scrubbing backwards
            int m = timeseries.Count - 1;
            if (m == -1) return;
            while (timeseries[m].time > (DateTime)lastDraw)
            {
                timeseries.RemoveAt(m);
                m = timeseries.Count - 1;
                if (m == -1) return;
            }

            // return if there are less than 2 records after culling
            int n = timeseries.Count;
            if (n < 2)
                return;

            // Build the new texture
            if (trendTexture == null)
            {
                Texture2D.Destroy(trendTexture);
                trendTexture = new Texture2D(width, height);
                texColors = new Color32[w * h];
                for (int i = 0; i < w * h; i++)
                    texColors[i] = Color.clear;

                trendTexture.wrapMode = TextureWrapMode.Clamp;
            }

            trendTexture.SetPixels32(texColors);

            // Loop through the timeseries records
            Vector2 prev = Record2Coords(timeseries[0]);
            for (int i = 0; i < timeseries.Count; i++)
            {
                Vector2 next = Record2Coords(timeseries[i]);
                Line(trendTexture, (int)prev.x, (int)prev.y, (int)next.x, (int)next.y, lineColor);
                prev = next;
            }

            // Apply to the world
            trendTexture.Apply();
            graphImage.sprite = Sprite.Create(trendTexture, new Rect(0, 0, width, height), new Vector2(0, 0));

        }

        /// <summary>
        /// Adds points on the texture that represent a straight line from the initial point
        /// to the ending point.
        /// </summary>
        /// <param name="tex">The texture that will take the pixel locations.</param>
        /// <param name="x0">The starting X location.</param>
        /// <param name="y0">The starting Y location.</param>
        /// <param name="x1">The ending X location.</param>
        /// <param name="y1">The ending Y location.</param>
        /// <param name="col">The color to make the pixel on the texture.</param>
        void Line(Texture2D tex, int x0, int y0, int x1, int y1, Color col)
        {
            int dy = y1 - y0;
            int dx = x1 - x0;
            int stepy, stepx;
            float fraction;

            if (dy < 0)
            {
                dy = -dy;
                stepy = -1;
            }
            else
            {
                stepy = 1;
            }

            if (dx < 0)
            {
                dx = -dx;
                stepx = -1;
            }
            else
            {
                stepx = 1;
            }

            dy <<= 1;
            dx <<= 1;

            tex.SetPixel(x0, y0, col);
            if (lineWidth > 1)
                tex.SetPixel(x0, y0 + 1, col);
            if (lineWidth > 2)
                tex.SetPixel(x0, y0 - 1, col);

            if (dx > dy)
            {
                fraction = dy - (dx >> 1);
                while (x0 != x1)
                {
                    if (fraction >= 0)
                    {
                        y0 += stepy;
                        fraction -= dx;
                    }
                    x0 += stepx;
                    fraction += dy;
                    tex.SetPixel(x0, y0, col);
                    if (lineWidth > 1)
                        tex.SetPixel(x0, y0 + 1, col);
                    if (lineWidth > 2)
                        tex.SetPixel(x0, y0 - 1, col);
                }
            }
            else
            {
                fraction = dx - (dy >> 1);
                while (y0 != y1)
                {
                    if (fraction >= 0)
                    {
                        x0 += stepx;
                        fraction -= dy;
                    }
                    y0 += stepy;
                    fraction += dx;
                    tex.SetPixel(x0, y0, col);
                    if (lineWidth > 1)
                        tex.SetPixel(x0, y0 + 1, col);
                    if (lineWidth > 2)
                        tex.SetPixel(x0, y0 - 1, col);
                }
            }
        }
        
        // converts a TimeseriesRecord to screen pixel coordinates for plotting
        Vector2 Record2Coords(TimeseriesRecord record)
        {
            float s = (float)(lastDraw - record.time).TotalSeconds;
            float normTime = Mathf.Clamp01(1 - s / timebase);
            float normHeight = Mathf.Clamp01((record.value - yMin) / (yMax - yMin));
            return new Vector2(w * normTime,
                               h * (1 - normHeight));
        }

        // add a time series to the trend graph
        public void Add(TimeseriesRecord record)
        {
            timeseries.Add(record);
            lastDraw = record.time;
            valueText.text = record.value.ToString(valueFormatString);
        }
        
        public void Add(DateTime time, float value)
        {
            Add(new TimeseriesRecord(time, value));
        }
    }
}
