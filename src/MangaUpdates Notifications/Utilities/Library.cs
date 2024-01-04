using MangaUpdates_Notifications.Models;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace MangaUpdates_Notifications.Utilities
{
    internal static class Library
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static ObservableCollection<Manga> LoadLibrary(string path)
        {
            ObservableCollection<Manga> library = new();

            if (!File.Exists(path))
            {
                Logger.Warn("The library file doesn't exist.");
                return library;
            }

            try
            {
                string json = File.ReadAllText(path);
                if (json.Length <= 2)
                    Logger.Warn("The library file appears to be empty.");
                else
                    library = JsonConvert.DeserializeObject<ObservableCollection<Manga>>(json) ?? library;
            }
            catch (Exception ex)
            {
                string message = "An error occured while loading the library.";
                _ = Misc.ShowDialog(message, ex);
                Logger.Error(ex, message);
            }

            return library;
        }

        public static void SaveLibrary(string path, ObservableCollection<Manga> collection)
        {
            string json = JsonConvert.SerializeObject(collection, new JsonSerializerSettings
                { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore});
            try
            {
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                string message = "An error occurred while saving the library.";
                _ = Misc.ShowDialog(message, ex);
                Logger.Error(ex, message);
            }
        }
    }
}
