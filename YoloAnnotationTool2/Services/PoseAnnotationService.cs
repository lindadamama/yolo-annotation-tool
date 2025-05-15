using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using YoloAnnotationTool2.ViewModels;
using Point = System.Windows.Point;

namespace YoloAnnotationTool2.Services
{
    public static class PoseAnnotationService
    {
        public static void SavePoseAnnotations(
            string labelPath,
            double canvasWidth,
            double canvasHeight,
            List<List<Point>> allPosePoints,
            string[] bodyParts
        )
        {
            labelPath = Path.ChangeExtension(labelPath, ".txt");

            using StreamWriter writer = new StreamWriter(labelPath);
            foreach (var pose in allPosePoints)
            {
                if (pose.Count != bodyParts.Length)
                    continue;

                double minX = pose.Min(p => p.X);
                double minY = pose.Min(p => p.Y);
                double maxX = pose.Max(p => p.X);
                double maxY = pose.Max(p => p.Y);

                double cx = (minX + maxX) / 2.0 / canvasWidth;
                double cy = (minY + maxY) / 2.0 / canvasHeight;
                double w = (maxX - minX) / canvasWidth;
                double h = (maxY - minY) / canvasHeight;

                List<string> parts = new()
                {
                    "0", // class index: 0 = "person"
                    cx.ToString("0.######", CultureInfo.InvariantCulture),
                    cy.ToString("0.######", CultureInfo.InvariantCulture),
                    w.ToString("0.######", CultureInfo.InvariantCulture),
                    h.ToString("0.######", CultureInfo.InvariantCulture)
                };

                foreach (var point in pose)
                {
                    double normX = point.X / canvasWidth;
                    double normY = point.Y / canvasHeight;

                    parts.Add(normX.ToString("0.######", CultureInfo.InvariantCulture));
                    parts.Add(normY.ToString("0.######", CultureInfo.InvariantCulture));
                    parts.Add("2"); // 2 = visible
                }

                writer.WriteLine(string.Join(" ", parts));
            }
        }

        public static void ClearPoseData(MainViewModel viewModel)
        {
            viewModel.PosePoints.Clear();
            viewModel.PoseKeypoints.Clear();
            viewModel.PoseSkeletonLines.Clear();
        }
    }
}
