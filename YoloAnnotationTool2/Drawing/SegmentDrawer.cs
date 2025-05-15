using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using YoloAnnotationTool2.ViewModels;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;
using Image = System.Windows.Controls.Image;

namespace YoloAnnotationTool2.Drawing
{
    public class SegmentDrawer : IAnnotationDrawer
    {
        private readonly MainViewModel _vm;
        private readonly Canvas _canvas;
        private readonly Image _image;
        private const double CloseThreshold = 25.0;

        public SegmentDrawer(MainViewModel vm, Canvas canvas, Image image)
        {
            _vm = vm;
            _canvas = canvas;
            _image = image;

            if (_vm._currentPolyline != null)
            {
                _canvas.Children.Remove(_vm._currentPolyline);
                _vm.drawnPolylines.Remove(_vm._currentPolyline);
            }

            _vm._currentPolyline = new Polyline
            {
                Stroke = _vm.ClassColors.ContainsKey(_vm.SelectedClassLabel)
                    ? _vm.ClassColors[_vm.SelectedClassLabel]
                    : Brushes.Red,
                StrokeThickness = 5
            };

            _vm._previewLine = new Line
            {
                Stroke = Brushes.Gray,
                StrokeThickness = 5,
                StrokeDashArray = new DoubleCollection { 2, 2 },
                Visibility = Visibility.Hidden,
                Tag = "PreviewLine"
            };

            _canvas.Children.Add(_vm._previewLine);
            _vm.drawnPolylines.Add(_vm._currentPolyline);
        }

        public void OnMouseDown(Point pos)
        {
            if (_vm._points.Count > 2 && IsCloseToFirstPoint(pos))
            {
                FinishSegment();
                return;
            }

            _vm._points.Add(pos);
            _vm._currentPolyline.Points.Add(pos);

            if (!_canvas.Children.Contains(_vm._currentPolyline))
            {
                _canvas.Children.Add(_vm._currentPolyline);
                _vm.drawnPolylines.Add(_vm._currentPolyline);
            }

            _vm._previewLine.Visibility = Visibility.Visible;
        }

        public void OnMouseMove(Point pos)
        {
            if (_vm._points.Count > 0)
            {
                Point lastPoint = _vm._points.Last();
                _vm._previewLine.X1 = lastPoint.X;
                _vm._previewLine.Y1 = lastPoint.Y;

                _vm._previewLine.X2 = pos.X;
                _vm._previewLine.Y2 = pos.Y;
                _vm._previewLine.Visibility = Visibility.Visible;
            }
            else
            {
                if (_canvas.Children.Contains(_vm._previewLine))
                {
                    _vm._previewLine.Visibility = Visibility.Hidden;
                }
            }
        }

        public void OnMouseUp(Point pos)
        {

        }

        private bool IsCloseToFirstPoint(Point point)
        {
            if (_vm._points.Count == 0)
                return false;

            Point first = _vm._points[0];
            double dx = point.X - first.X;
            double dy = point.Y - first.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            return distance < CloseThreshold;
        }
        private void FinishSegment()
        {
            Polygon polygon = new Polygon
            {
                Stroke = _vm.ClassColors.ContainsKey(_vm.SelectedClassLabel)
                    ? _vm.ClassColors[_vm.SelectedClassLabel]
                    : Brushes.Red,
                StrokeThickness = 2,
                Fill = _vm.ClassColors.ContainsKey(_vm.SelectedClassLabel)
                    ? new SolidColorBrush(((SolidColorBrush)_vm.ClassColors[_vm.SelectedClassLabel]).Color) { Opacity = 0.5 }
                    : Brushes.Red,
                Points = new PointCollection(_vm._points),
                Tag = _vm.SelectedClassLabel
            };

            if (_vm._currentPolyline != null)
            {
                _canvas.Children.Remove(_vm._currentPolyline);
                _vm.drawnPolylines.Remove(_vm._currentPolyline);
            }

            if (_vm._previewLine != null)
            {
                _vm._previewLine.Visibility = Visibility.Hidden;

                if (_canvas.Children.Contains(_vm._previewLine))
                {
                    _canvas.Children.Remove(_vm._previewLine);
                }

                _vm._previewLine = null;
            }

            _vm.drawnPolylines.Clear();

            _canvas.Children.Add(polygon);
            _vm.drawnPolygons.Add(polygon);

            _vm._points.Clear();
            _canvas.InvalidateVisual();
            _canvas.UpdateLayout();
            for (int i = _canvas.Children.Count - 1; i >= 0; i--)
            {
                if (_canvas.Children[i] is Line line && line.Tag as string == "PreviewLine")
                {
                    _canvas.Children.RemoveAt(i);
                }
            }
        }
    }
}
