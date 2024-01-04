using MangaUpdates_Notifications.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MangaUpdates_Notifications.Utilities
{
    internal static class Misc
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static async Task ShowDialog(string message, Exception? e = null)
        {
            await Global.DialogSemaphore.WaitAsync();

            Global.MainView.Visibility = Visibility.Visible;

            Dialog dialog = new() { MainMessage = message, ExceptionString = e?.ToString() };
            await dialog.ShowAsync();

            Global.DialogSemaphore.Release();
        }

        public static List<string> GetColorNames()
        {
            PropertyInfo[] colorProperties = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static);

            return (from property in colorProperties where property.PropertyType == typeof(Color) select property.Name).ToList();
        }

        public static async Task<BitmapImage?> UrlToBitmapImg(string url)
        {
            BitmapImage? bitmapImage = null;
            byte[] imageData = await Cache.GetCachedData(url);
            if (imageData.Length == 0)
            {
                try
                {
                    imageData = await Global.HttpClient.GetByteArrayAsync(url);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Failed to get the image \"{url}\".");
                }
            }
            if (imageData.Length > 0)
            {
                await Cache.CacheData(url, imageData);

                using MemoryStream memoryStream = new MemoryStream(imageData);
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }

        public static void DeleteOldFiles(string path, int days)
        {
            if (!Directory.Exists(path))
                return;

            try
            {
                string[] files = Directory.GetFiles(path);
                if (files.Length > 0)
                {
                    foreach (var file in files)
                    {
                        DateTime createdTime = File.GetCreationTime(file);
                        TimeSpan passedTime = DateTime.Now - createdTime;
                        if (passedTime.Days >= days)
                            File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An error occured while deleting the old files.");
            }
        }
    }
}