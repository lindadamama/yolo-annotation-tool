using System.IO;
using YoloAnnotationTool2.Enums;
using YoloAnnotationTool2.ViewModels;
using System.Linq;
using System.Windows;
using System;

namespace YoloAnnotationTool2.Services
{
    public static class ProjectService
    {
        public static void CreateProjectStructure(string path, string suffix, AnnotationType type, string projectName)
        {
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(Path.Combine(path, "images", "train"));
            Directory.CreateDirectory(Path.Combine(path, "images", "val"));
            Directory.CreateDirectory(Path.Combine(path, "labels", "train"));
            Directory.CreateDirectory(Path.Combine(path, "labels", "val"));

            string yamlPath = Path.Combine(path, projectName + ".yaml");

            if (type == AnnotationType.Pose)
            {
                string poseContent = "train: images/train\nval: images/val\r\n\r\nkpt_shape: [17, 3]  \r\nflip_idx: [0, 2, 1, 4, 3, 6, 5, 8, 7, 10, 9, 12, 11, 14, 13, 16, 15]\r\n\r\nnames:\n  0: person";
                File.WriteAllText(yamlPath, poseContent);
            }
            else
            {
                string content = "train: images/train\nval: images/val\n\nnames:";
                File.WriteAllText(yamlPath, content);
            }
        }

        public static void CreateClassifyProjectStructure(string projectPath)
        {
            Directory.CreateDirectory(Path.Combine(projectPath, "train"));
            Directory.CreateDirectory(Path.Combine(projectPath, "val"));
        }

        public static void LoadClassesClassify(MainViewModel viewModel, string projectPath)
        {
            string classDirectoriesPath = Path.Combine(projectPath, "train");
            if (Directory.Exists(classDirectoriesPath))
            {
                string[] directories = Directory.GetDirectories(classDirectoriesPath);

                foreach (string dir in directories)
                {
                    string folderName = Path.GetFileName(dir);
                    viewModel.ClassLabels.Add(folderName);
                }
            }
        }

        public static string GetAnnotationSuffix(MainViewModel viewModel)
        {
            if (viewModel.IsOptionDetectChecked) return "_Detect";
            if (viewModel.IsOptionOBBChecked) return "_OBB";
            if (viewModel.IsOptionSegmentChecked) return "_Segment";
            if (viewModel.IsOptionClassifyChecked) return "_Classify";
            if (viewModel.IsOptionPoseChecked) return "_Pose";
            return null;
        }

        public static bool EndsWithDetect(string input) => input?.EndsWith("_Detect", StringComparison.OrdinalIgnoreCase) == true;
        public static bool EndsWithOBB(string input) => input?.EndsWith("_OBB", StringComparison.OrdinalIgnoreCase) == true;
        public static bool EndsWithSegment(string input) => input?.EndsWith("_Segment", StringComparison.OrdinalIgnoreCase) == true;
        public static bool EndsWithClassify(string input) => input?.EndsWith("_Classify", StringComparison.OrdinalIgnoreCase) == true;
        public static bool EndsWithPose(string input) => input?.EndsWith("_Pose", StringComparison.OrdinalIgnoreCase) == true;
    }
}
