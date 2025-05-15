using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoloAnnotationTool2.Models
{
    public class KeyPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int Visibility { get; set; }

        public KeyPoint(double x, double y, int visibility)
        {
            X = x;
            Y = y;
            Visibility = visibility;
        }
    }

}
