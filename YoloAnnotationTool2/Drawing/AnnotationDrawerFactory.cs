using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using YoloAnnotationTool2.Enums;
using YoloAnnotationTool2.ViewModels;

namespace YoloAnnotationTool2.Drawing
{
    public static class AnnotationDrawerFactory
    {
        public static IAnnotationDrawer CreateDrawer(
            AnnotationType type,
            MainViewModel vm,
            Canvas canvas,
            System.Windows.Controls.Image image)
        {
            return type switch
            {
                AnnotationType.Detect => new BBoxDrawer(vm, canvas, image),
                AnnotationType.OBB => new OBBDrawer(vm, canvas, image),
                AnnotationType.Segment => new SegmentDrawer(vm, canvas, image),
                AnnotationType.Pose => new PoseDrawer(vm, canvas, image),
                _ => null
            };
        }
    }

}
