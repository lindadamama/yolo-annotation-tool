using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using YoloAnnotationTool2.ViewModels;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace YoloAnnotationTool2.Drawing
{
    public class OBBDrawer : IAnnotationDrawer
    {
        private readonly MainViewModel _vm;
        private readonly Canvas _canvas;
        private readonly Image _image;
        private Point _startPoint;
        private Rectangle _currentRect;
        private RotateTransform _rotateTransform;
        private Point? _secondVector = null;
        private double _fixedAngle = 0;
        private double _fixedWidth = 0;

        public OBBDrawer(MainViewModel vm, Canvas canvas, Image image)
        {
            _vm = vm;
            _canvas = canvas;
            _image = image;
        }

        public void OnMouseDown(Point pos)
        {
            if (pos.X < 0 || pos.Y < 0 || pos.X > _canvas.ActualWidth || pos.Y > _canvas.ActualHeight)
                return;

            _startPoint = pos;

            _currentRect = new Rectangle
            {
                Stroke = _vm.ClassColors.ContainsKey(_vm.SelectedClassLabel)
                    ? _vm.ClassColors[_vm.SelectedClassLabel]
                    : Brushes.Red,
                StrokeThickness = 2,
                Width = 0,
                Height = 0
            };

            Canvas.SetLeft(_currentRect, pos.X);
            Canvas.SetTop(_currentRect, pos.Y);

            _canvas.Children.Add(_currentRect);
            _vm.drawnRectangles.Add(_currentRect);
        }

        public void OnMouseMove(Point pos)
        {
            if (_currentRect == null) return;

            if (pos.X < 0 || pos.Y < 0 || pos.X > _canvas.ActualWidth || pos.Y > _canvas.ActualHeight)
                return;

            Vector delta = pos - _startPoint;
            double angle;
            double length = delta.Length;

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                if (pos.X < 0 || pos.Y < 0 || pos.X > _canvas.ActualWidth || pos.Y > _canvas.ActualHeight)
                    return;
                if (_secondVector == null)
                {

                    _fixedAngle = Math.Atan2(delta.Y, delta.X) * 180 / Math.PI;
                    _fixedWidth = length;
                    _secondVector = pos;

                    _rotateTransform = new RotateTransform(_fixedAngle, 0, 0);
                    _currentRect.RenderTransform = _rotateTransform;

                    _currentRect.Width = _fixedWidth;
                }
                else
                {
                    Vector heightVector = pos - _secondVector.Value;

                    _currentRect.Width = _fixedWidth;
                    _currentRect.Height = heightVector.Length;

                }
            }
            else
            {
                angle = Math.Atan2(delta.Y, delta.X) * 180 / Math.PI;

                _rotateTransform = new RotateTransform(angle, 0, 0);
                _currentRect.RenderTransform = _rotateTransform;

                _secondVector = null;
                _fixedWidth = 0;

                _currentRect.Width = length;
                _currentRect.Height = 10; 
            }


            Canvas.SetLeft(_currentRect, _startPoint.X);
            Canvas.SetTop(_currentRect, _startPoint.Y);
        }

        public void OnMouseUp(Point pos)
        {
            if (_currentRect == null) return;

            _currentRect.Tag = _vm.SelectedClassLabel;

            if (_currentRect.Width < 5 || _currentRect.Height < 5)
            {
                _canvas.Children.Remove(_currentRect);
                _vm.drawnRectangles.Remove(_currentRect);
            }

            _currentRect = null;
            _rotateTransform = null;
            _secondVector = null;
        }
    }
}
