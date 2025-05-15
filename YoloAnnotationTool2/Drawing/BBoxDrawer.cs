using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using YoloAnnotationTool2.ViewModels;
using System.Windows.Input;

namespace YoloAnnotationTool2.Drawing
{
    public class BBoxDrawer : IAnnotationDrawer
    {
        private readonly MainViewModel _vm;
        private readonly Canvas _canvas;
        private readonly System.Windows.Controls.Image _image;

        private System.Windows.Point _start;
        private System.Windows.Shapes.Rectangle _currentRect;

        public BBoxDrawer(MainViewModel vm, Canvas canvas, System.Windows.Controls.Image image)
        {
            _vm = vm;
            _canvas = canvas;
            _image = image;
        }

        public void OnMouseDown(System.Windows.Point pos)
        {
            _start = pos;
            _currentRect = new System.Windows.Shapes.Rectangle
            {
                Stroke = _vm.ClassColors.ContainsKey(_vm.SelectedClassLabel) ? _vm.ClassColors[_vm.SelectedClassLabel] : System.Windows.Media.Brushes.Red,
                StrokeThickness = 2
            };
            Canvas.SetLeft(_currentRect, pos.X);
            Canvas.SetTop(_currentRect, pos.Y);
            _canvas.Children.Add(_currentRect);
            _vm.drawnRectangles.Add(_currentRect);
        }

        public void OnMouseMove(System.Windows.Point pos)
        {
            if (_currentRect == null) return;

            double x = Math.Min(pos.X, _start.X);
            double y = Math.Min(pos.Y, _start.Y);
            double w = Math.Abs(pos.X - _start.X);
            double h = Math.Abs(pos.Y - _start.Y);

            Canvas.SetLeft(_currentRect, x);
            Canvas.SetTop(_currentRect, y);
            _currentRect.Width = w;
            _currentRect.Height = h;
        }

        public void OnMouseUp(System.Windows.Point pos)
        {
            if (_currentRect == null) return;

            _currentRect.Tag = _vm.SelectedClassLabel;
            _currentRect = null;
        }

    }

}
