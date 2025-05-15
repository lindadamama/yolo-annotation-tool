using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using YoloAnnotationTool2.Enums;
using YoloAnnotationTool2.ViewModels;
using System.Windows.Shapes;


using Point = System.Windows.Point;
using Brush = System.Windows.Media.Brush;
using Rectangle = System.Windows.Shapes.Rectangle;
using Path = System.IO.Path;


namespace YoloAnnotationTool2.Services
{
    public static class AnnotationService
    {
        public static void SaveAnnotations(
            string labelPath,
            List<UIElement> annotations,
            AnnotationType annotationType,
            Dictionary<string, Brush> classColors,
            IList<string> classLabels,
            double canvasWidth,
            double canvasHeight)
        {
            labelPath = Path.ChangeExtension(labelPath, ".txt");


            var uniqueAnnotations = new HashSet<string>();
            using StreamWriter writer = new StreamWriter(labelPath, false); 
            foreach (var element in annotations)
            {
                string annotation = annotationType switch
                {
                    AnnotationType.Detect => FormatBBoxAnnotation(element, classLabels, canvasWidth, canvasHeight),
                    AnnotationType.OBB => FormatOBBAnnotation(element, classLabels, canvasWidth, canvasHeight),
                    AnnotationType.Segment => FormatSegmentAnnotation(element, classLabels, canvasWidth, canvasHeight),
                    _ => null
                };

                if (!string.IsNullOrEmpty(annotation) && uniqueAnnotations.Add(annotation))
                {
                    writer.WriteLine(annotation);
                }

            }
        }


        public static string FormatBBoxAnnotation(UIElement element, IList<string> classLabels, double canvasWidth, double canvasHeight)
        {
            if (element is not Rectangle rect) return null;

            double x = Canvas.GetLeft(rect);
            double y = Canvas.GetTop(rect);
            double width = rect.Width;
            double height = rect.Height;

            double centerX = x + width / 2;
            double centerY = y + height / 2;

            double normCenterX = centerX / canvasWidth;
            double normCenterY = centerY / canvasHeight;
            double normWidth = width / canvasWidth;
            double normHeight = height / canvasHeight;

            if (normCenterX > 1.0 || normCenterY > 1.0 || normWidth > 1.0 || normHeight > 1.0)
                return null;

            int classIndex = classLabels.IndexOf(rect.Tag as string);
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} {1:0.######} {2:0.######} {3:0.######} {4:0.######}",
                classIndex, normCenterX, normCenterY, normWidth, normHeight
            );
        }

        public static string FormatSegmentAnnotation(UIElement element, IList<string> classLabels, double canvasWidth, double canvasHeight)
        {
            if (element is not Polygon polygon || polygon.Points.Count < 3)
                return null;

            int classIndex = classLabels.IndexOf(polygon.Tag as string);
            if (classIndex == -1) return null;

            List<string> normalizedPoints = new();

            foreach (Point pt in polygon.Points)
            {
                double normX = pt.X / canvasWidth;
                double normY = pt.Y / canvasHeight;

                if (normX < 0 || normX > 1 || normY < 0 || normY > 1)
                    return null;

                normalizedPoints.Add(string.Format(CultureInfo.InvariantCulture, "{0:0.######} {1:0.######}", normX, normY));
            }

            return $"{classIndex} {string.Join(" ", normalizedPoints)}";
        }
        public static string FormatOBBAnnotation(UIElement element, IList<string> classLabels, double canvasWidth, double canvasHeight)
        {
            if (element is Polygon polygon && polygon.Points.Count == 4)
            {
                int classIndex = classLabels.IndexOf(polygon.Tag as string);
                if (classIndex < 0) return null;

                var normalizedPoints = polygon.Points.Select(p => new Point(
                    p.X / canvasWidth,
                    p.Y / canvasHeight)).ToList();

                if (normalizedPoints.Any(p => p.X < 0 || p.X > 1 || p.Y < 0 || p.Y > 1))
                    return null;

                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} {1:0.######} {2:0.######} {3:0.######} {4:0.######} {5:0.######} {6:0.######} {7:0.######} {8:0.######}",
                    classIndex,
                    normalizedPoints[0].X, normalizedPoints[0].Y,
                    normalizedPoints[1].X, normalizedPoints[1].Y,
                    normalizedPoints[2].X, normalizedPoints[2].Y,
                    normalizedPoints[3].X, normalizedPoints[3].Y
                );
            }

            if (element is Rectangle rect)
            {
                double left = Canvas.GetLeft(rect);
                double top = Canvas.GetTop(rect);
                double width = rect.Width;
                double height = rect.Height;

                double angle = 0;
                if (rect.RenderTransform is RotateTransform rotateTransform)
                {
                    angle = rotateTransform.Angle;
                }

                double radians = angle * Math.PI / 180;
                double cos = Math.Cos(radians);
                double sin = Math.Sin(radians);

                Point[] localCorners = new Point[]
                {
            new(0, 0),
            new(width, 0),
            new(width, height),
            new(0, height)
                };

                Point[] absoluteCorners = new Point[4];

                for (int i = 0; i < 4; i++)
                {
                    double x = localCorners[i].X;
                    double y = localCorners[i].Y;

                    double rotatedX = x * cos - y * sin;
                    double rotatedY = x * sin + y * cos;

                    double canvasX = left + rotatedX;
                    double canvasY = top + rotatedY;

                    double normX = canvasX / canvasWidth;
                    double normY = canvasY / canvasHeight;

                    if (normX < 0 || normX > 1 || normY < 0 || normY > 1)
                        return null;

                    absoluteCorners[i] = new Point(normX, normY);
                }

                int classIndex = classLabels.IndexOf(rect.Tag as string);
                if (classIndex < 0) return null;

                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} {1:0.######} {2:0.######} {3:0.######} {4:0.######} {5:0.######} {6:0.######} {7:0.######} {8:0.######}",
                    classIndex,
                    absoluteCorners[0].X, absoluteCorners[0].Y,
                    absoluteCorners[1].X, absoluteCorners[1].Y,
                    absoluteCorners[2].X, absoluteCorners[2].Y,
                    absoluteCorners[3].X, absoluteCorners[3].Y
                );
            }

            return null;
        }


    }
}
