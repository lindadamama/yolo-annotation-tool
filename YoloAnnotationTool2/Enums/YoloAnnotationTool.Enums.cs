using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoloAnnotationTool2.Enums
{
    public enum AnnotationType
    {
        Detect,     // BBox
        OBB,        // Oriented Bounding Box
        Segment,    // Polygon
        Pose,       // Keypoints
        Classify,   // Classify
        Empty
    }
}
