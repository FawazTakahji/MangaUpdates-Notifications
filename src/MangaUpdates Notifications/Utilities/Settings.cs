using Newtonsoft.Json;
using System;
using System.IO;
using ModernWpf;

namespace MangaUpdates_Notifications.Utilities
{
    internal static class Settings
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static Models.Settings LoadSettings(string path)
        {
            Models.Settings settings = new ();

            if (!File.Exists(path))
            {
                string message = "The settings file doesn't exist, a new file will be created.";
                Logger.Warn(message);
                SaveSettings(path, settings);
                return settings;
            }

            try
            {
                string json = File.ReadAllText(path);
                if (json.Length <= 2)
                {
                    string message = "The settings file appears to be empty, a new file will be created.";
                    Logger.Warn(message);
                    SaveSettings(path, settings);
                }
                else
                    settings = JsonConvert.DeserializeObject<Models.Settings>(json) ?? settings;
            }
            catch (Exception ex)
            {
                string message = "An error occurred while loading the app settings, using default values.";
                _ = Misc.ShowDialog(message, ex);
                Logger.Error(ex, message);
            }

            if (!Enum.IsDefined(typeof(ApplicationTheme), settings.Personalization.Theme))
                settings.Personalization.Theme = ApplicationTheme.Dark;

            return settings;
        }

        public static void SaveSettings(string path, Models.Settings settings)
        {
            try
            {
                File.WriteAllText(path, settings.Json);
            }
            catch (Exception ex)
            {
                string message = "An error occurred while saving the app settings.";
                _ = Misc.ShowDialog(message, ex);
                Logger.Error(ex, message);
            }
        }

        public static void ApplySettings(Models.Settings settings)
        {
            ThemeManager.Current.ApplicationTheme = settings.Personalization.Theme;
            ThemeManager.Current.AccentColor = settings.Personalization.Accent;
            Notifications.CheckTimer.Interval = settings.Personalization.Interval.Milliseconds;
            Global.Settings = settings;
        }
    }
}