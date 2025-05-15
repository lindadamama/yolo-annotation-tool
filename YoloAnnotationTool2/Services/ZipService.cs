using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using YoloAnnotationTool2.Enums;
using YoloAnnotationTool2.ViewModels;
using MessageBox = System.Windows.MessageBox;

namespace YoloAnnotationTool2.Services
{
    public static class ZipService
    {
        public static void CreateZipArchive(string directoryPath, string projectWithSuffix, AnnotationType type)
        {
            string directoryName = new DirectoryInfo(directoryPath).Name;
            string zipFilePath = Path.Combine(Directory.GetParent(directoryPath).FullName, directoryName + ".zip");

            if (type == AnnotationType.Classify)
            {
                CreateClassifyZipArchive(directoryPath, zipFilePath);
                return;
            }

            bool isTestExist = Directory.Exists(Path.Combine(directoryPath, "images", "test"));

            if (type == AnnotationType.Pose && isTestExist)
            {
                string yamlPath = Directory.GetFiles(directoryPath, "*.yaml").FirstOrDefault();
                if (!string.IsNullOrEmpty(yamlPath))
                {
                    var lines = "train: images/train\nval: images/val\ntest: images/test\r\n\r\nkpt_shape: [17, 3]  \r\nflip_idx: [0, 2, 1, 4, 3, 6, 5, 8, 7, 10, 9, 12, 11, 14, 13, 16, 15]\r\n\r\nnames:\n  0: person";
                    File.WriteAllText(yamlPath, lines);
                }
            }

            try
            {
                if (File.Exists(zipFilePath))
                    File.Delete(zipFilePath);

                ZipFile.CreateFromDirectory(directoryPath, zipFilePath, CompressionLevel.Fastest, true);

                MessageBox.Show($"Archive created: {zipFilePath}");

                bool valid = CheckYoloDataset(zipFilePath, projectWithSuffix);
                MessageBox.Show(valid ? "Dataset is valid!" : "Error in dataset");

                RemoveFilesFromZip(zipFilePath, new[] { "colors.json", "class_labels.json" });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        public static void CreateClassifyZipArchive(string directoryPath, string zipFilePath)
        {
            try
            {
                if (File.Exists(zipFilePath))
                    File.Delete(zipFilePath);

                ZipFile.CreateFromDirectory(directoryPath, zipFilePath, CompressionLevel.Fastest, true);
                MessageBox.Show($"Archive created: {zipFilePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        public static void RemoveFilesFromZip(string zipFilePath, string[] excludeFileNames)
        {
            try
            {
                using var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Update);
                var entriesToRemove = archive.Entries
                    .Where(entry => excludeFileNames.Contains(Path.GetFileName(entry.FullName)))
                    .ToList();

                foreach (var entry in entriesToRemove)
                    entry.Delete();

                MessageBox.Show("Service files are successfully deleted from the archive.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error when deleting service files from the archive: {ex.Message}");
            }
        }

        public static bool CheckYoloDataset(string zipPath, string projectWithSuffix)
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            try
            {
                ZipFile.ExtractToDirectory(zipPath, tempFolder);

                string yamlPath = Directory.GetFiles(tempFolder, projectWithSuffix + ".yaml", SearchOption.AllDirectories).FirstOrDefault();
                if (yamlPath == null)
                {
                    MessageBox.Show("File not found data.yaml", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                string[] requiredDirs = {
                    "images/train", "images/val",
                    "labels/train", "labels/val"
                };

                foreach (string dir in requiredDirs)
                {
                    string fullPath = Path.Combine(tempFolder, projectWithSuffix, dir.Replace('/', Path.DirectorySeparatorChar));
                    if (!Directory.Exists(fullPath))
                    {
                        MessageBox.Show($"Required directory not found: {dir}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }

                var labelFiles = Directory.GetFiles(Path.Combine(tempFolder, projectWithSuffix, "labels/train"), "*.txt");
                if (labelFiles.Length == 0)
                {
                    MessageBox.Show("No .txt files with annotations found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                foreach (var labelFile in labelFiles)
                {
                    foreach (var line in File.ReadLines(labelFile))
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        var parts = line.Split(' ');

                        if (projectWithSuffix.EndsWith("_Detect"))
                        {
                            if (parts.Length != 5 || !int.TryParse(parts[0], out _) ||
                                !parts.Skip(1).Take(4).All(p => double.TryParse(p, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _)))
                            {
                                MessageBox.Show($"Incorrect data in the detect: {line}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return false;
                            }
                        }
                        else if (projectWithSuffix.EndsWith("_OBB") && parts.Length != 9)
                        {
                            MessageBox.Show($"Incorrect data in the obb: {line}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking the dataset: {ex.Message}");
                return false;
            }
            finally
            {
                try { if (Directory.Exists(tempFolder)) Directory.Delete(tempFolder, true); } catch { }
            }
        }
    }
}
