using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using CommonGenius.Collections;

namespace VTL.TrendGraph
{
    public class TrendGraphController : MonoBehaviour
    {
        public float Ymax = 1;
        public float Ymin = 0;
        public float Timebase = 600;
        public string TimebaseLabel = "-5 min";

        OrderedDictionary<DateTime, float> timeseries;

        RectTransform rectTransform;

        void OnValidate()
        {
            transform.Find("Ymax")
                     .GetComponent<Text>()
                     .text = Ymax.ToString();

            transform.Find("Ymin")
                     .GetComponent<Text>()
                     .text = Ymin.ToString();

            transform.Find("Timebase")
                     .GetComponent<Text>()
                     .text = TimebaseLabel;
        }

        // Use this for initialization
        void Start()
        {
            timeseries = new OrderedDictionary<DateTime, float>();

            rectTransform = transform.Find("Graph") as RectTransform;

        }
        void OnGUI()
        {
            var origin = rectTransform.position;
            origin.y = Screen.height - origin.y;

            float w = origin.x + rectTransform.rect.width;
            float h = origin.y + rectTransform.rect.height;

            Drawing.DrawLine(origin, new Vector2(origin.x + w, origin.y + h), Color.white, 1f);

        }

        void Add(DateTime time, float value)
        {
            timeseries.Add(time, value);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
