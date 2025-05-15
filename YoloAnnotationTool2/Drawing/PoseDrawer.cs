using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using YoloAnnotationTool2.ViewModels;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace YoloAnnotationTool2.Drawing
{
    public class PoseDrawer : IAnnotationDrawer
    {
        private readonly MainViewModel _vm;
        private readonly Canvas _canvas;
        private readonly Image _image;

        private readonly string[] _bodyParts = new string[]
        {
        "Nose", "Left Eye", "Right Eye", "Left Ear", "Right Ear",
        "Left Shoulder", "Right Shoulder", "Left Elbow", "Right Elbow",
        "Left Wrist", "Right Wrist", "Left Hip", "Right Hip",
        "Left Knee", "Right Knee", "Left Ankle", "Right Ankle"
        };

        private readonly int[][] _skeletonConnections = new int[][]
        {
        new[] {5, 6}, new[] {5, 7}, new[] {7, 9}, new[] {6, 8}, new[] {8, 10},
        new[] {11, 12}, new[] {5, 11}, new[] {6, 12}, new[] {11, 13}, new[] {13, 15},
        new[] {12, 14}, new[] {14, 16}, new[] {0, 1}, new[] {0, 2}, new[] {1, 3},
        new[] {2, 4}, new[] {0, 5}, new[] {0, 6}
        };

        public PoseDrawer(MainViewModel vm, Canvas canvas, Image image)
        {
            _vm = vm;
            _canvas = canvas;
            _image = image;

            UpdateCounterText();
        }

        public void OnMouseDown(Point pos)
        {
            if (_vm.PoseCurrentPointIndex >= 17)
                return;

            _vm.PosePoints.Add(pos);

            var ellipse = new Ellipse
            {
                Width = 6,
                Height = 6,
                Fill = Brushes.Yellow,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            Canvas.SetLeft(ellipse, pos.X - 3);
            Canvas.SetTop(ellipse, pos.Y - 3);
            _canvas.Children.Add(ellipse);
            _vm.PoseKeypoints.Add(ellipse);

            foreach (var conn in _skeletonConnections)
            {
                if ((conn[0] == _vm.PoseCurrentPointIndex || conn[1] == _vm.PoseCurrentPointIndex) &&
                    conn[0] < _vm.PosePoints.Count && conn[1] < _vm.PosePoints.Count)
                {
                    var pt1 = _vm.PosePoints[conn[0]];
                    var pt2 = _vm.PosePoints[conn[1]];

                    var line = new Line
                    {
                        X1 = pt1.X,
                        Y1 = pt1.Y,
                        X2 = pt2.X,
                        Y2 = pt2.Y,
                        Stroke = Brushes.LimeGreen,
                        StrokeThickness = 2
                    };
                    _canvas.Children.Add(line);
                    _vm.PoseSkeletonLines.Add(line);
                }
            }

            _vm.PoseCurrentPointIndex++;
            UpdateCounterText();

            if (_vm.PoseCurrentPointIndex == 17)
            {
                var group = new Canvas();

                foreach (var e in _vm.PoseKeypoints)
                {
                    _canvas.Children.Remove(e);
                    group.Children.Add(e);
                }
                foreach (var l in _vm.PoseSkeletonLines)
                {
                    _canvas.Children.Remove(l);
                    group.Children.Add(l);
                }

                _canvas.Children.Add(group);

                _vm.AllPosePoints.Add(new List<Point>(_vm.PosePoints)); // копія
                _vm.PosePoints.Clear();
                _vm.PoseKeypoints.Clear();
                _vm.PoseSkeletonLines.Clear();
                _vm.PoseCurrentPointIndex = 0;
            }
        }
        public void OnMouseMove(Point pos) { }
        public void OnMouseUp(Point pos) { }
        private void UpdateCounterText()
        {
            if (_vm.PoseCurrentPointIndex < _bodyParts.Length)
                _vm.pointCounterText = $"Point {_vm.PoseCurrentPointIndex + 1}/17: {_bodyParts[_vm.PoseCurrentPointIndex]}";
            else
                _vm.pointCounterText = "Pose complete. For the next pose start drawing from: Nose";
        }
    }

}
