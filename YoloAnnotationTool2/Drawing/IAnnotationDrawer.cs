using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace YoloAnnotationTool2.Drawing
{
    public interface IAnnotationDrawer
    {
        void OnMouseDown(System.Windows.Point point);
        void OnMouseMove(System.Windows.Point point);
        void OnMouseUp(System.Windows.Point point);

    }

}
