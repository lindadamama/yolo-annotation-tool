using System;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Forms;

public class AppSettings
{
    public string SaveFolderPath { get; set; }
    public string CurrentProjectPath { get; set; }
}

public static class SettingsManager
{
    private static string _cachedSettingsPath;

    public static AppSettings Load()
    {
        if (string.IsNullOrWhiteSpace(_cachedSettingsPath))
        {
            _cachedSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
        }

        if (!File.Exists(_cachedSettingsPath))
            return new AppSettings();

        var json = File.ReadAllText(_cachedSettingsPath);
        return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
    }

    public static void Save(AppSettings settings)
    {
        if (string.IsNullOrWhiteSpace(_cachedSettingsPath))
        {
            _cachedSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
        }

        var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
        File.WriteAllText(_cachedSettingsPath, json);
    }

    public static string ChooseSaveFolderPath()
    {
        using (var dialog = new FolderBrowserDialog())
        {
            dialog.Description = "Select a directory to save projects in";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
            else
            {
                MessageBox.Show("No directory is selected");
                return null;
            }
        }
    }
}
