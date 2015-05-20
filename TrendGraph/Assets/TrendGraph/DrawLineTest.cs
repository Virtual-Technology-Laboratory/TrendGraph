using UnityEngine;
using System.Collections;

namespace VTL.TrendGraph
{
    public class DrawLineTest : MonoBehaviour
    {

        // Use this for initialization
        void OnGUI()
        {
            var a = new Vector2(0, 0);
            var b = new Vector2(1000, 1000);
            Drawing.DrawLine(a, b, Color.white, 0.1f);

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}