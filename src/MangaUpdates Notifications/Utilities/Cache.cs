using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MangaUpdates_Notifications.Utilities
{
    internal static class Cache
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static string CacheFolder { get; } = Path.Combine(Global.BaseDir, "Cache");
        private static readonly System.Timers.Timer CleanupTimer;
        private static readonly SemaphoreSlim CacheAccess = new(1, 1);

        static Cache()
        {
            CleanupTimer = new System.Timers.Timer(TimeSpan.FromHours(1));
            CleanupTimer.Elapsed += async (_, _) => await DeleteOldFiles();
            CleanupTimer.Start();
        }

        public static async Task<byte[]> GetCachedData(string url)
        {
            await CacheAccess.WaitAsync();
            byte[] data = Array.Empty<byte>();
            string filePath = Path.Combine(CacheFolder, UrlEncoder(url));
            try
            {
                if (File.Exists(filePath))
                    data = await File.ReadAllBytesAsync(filePath);
                else
                    Logger.Warn($"Cache for \"{url}\" doesn't exist.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"An error occured while loading the data from \"{filePath}\".");
            }
            finally
            {
                CacheAccess.Release();
            }

            return data;
        }

        public static async Task CacheData(string url, byte[] data)
        {
            await CacheAccess.WaitAsync();
            string filePath = Path.Combine(CacheFolder, UrlEncoder(url));
            try
            {
                Directory.CreateDirectory(CacheFolder);
                await File.WriteAllBytesAsync(filePath, data);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"An error occured while writing to \"{filePath}\".");
            }
            finally
            {
                CacheAccess.Release();
            }
        }

        public static async Task DeleteOldFiles()
        {
            await CacheAccess.WaitAsync();
            try
            {
                if (!Directory.Exists(CacheFolder))
                    return;

                string[] files = Directory.GetFiles(CacheFolder);
                if (files.Length > 0)
                {
                    foreach (var file in files)
                    {
                        DateTime modifiedTime = File.GetLastWriteTime(file);
                        TimeSpan passedTime = DateTime.Now - modifiedTime;
                        if (passedTime.TotalDays >= 1)
                            File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An error occured while deleting the old cache files.");
            }
            finally
            {
                CacheAccess.Release();
            }
        }

        private static string UrlEncoder(string originalUrl)
        {
            string url = Regex.Replace(originalUrl, @"^https?://", string.Empty);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(url));
        }
    }
}
