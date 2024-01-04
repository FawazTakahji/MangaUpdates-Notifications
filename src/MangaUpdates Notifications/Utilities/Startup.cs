using System;
using System.IO;
using System.Linq;
using MangaUpdates_Notifications.Models;
using Microsoft.Win32;
using NLog;

namespace MangaUpdates_Notifications.Utilities
{
    public static class Startup
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void AddToStartup(bool portable, bool minimized)
        {
            string name = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
            string value = $"\"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MangaUpdates Notifications.exe")}\"";
            if (portable)
                value += " -p";
            if (minimized)
                value += " -m";
            try
            {
                RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (key != null)
                    key.SetValue(name, value);
                else
                {
                    string message = "Failed to add the app to the startup list.";
                    _ = Misc.ShowDialog(message);
                    Logger.Error(message);
                }
            }
            catch (Exception ex)
            {
                string message = "An error occured while adding the app to the startup list.";
                _ = Misc.ShowDialog(message, ex);
                Logger.Error(ex, message);
            }
        }

        public static void RemoveFromStartup()
        {
            string name = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
            try
            {
                RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (key != null)
                {
                    if (key.GetValueNames().Contains(name))
                        key.DeleteValue(name);
                }
                else
                {
                    string message = "Failed to remove the app from the startup list.";
                    _ = Misc.ShowDialog(message);
                    Logger.Error(message);
                }
            }
            catch (Exception ex)
            {
                string message = "An error occured while removing the app from the startup list.";
                _ = Misc.ShowDialog(message, ex);
                Logger.Error(ex, message);
            }
        }

        public static StartupInfo GetStartupInfo()
        {
            string name = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
            string value = $"\"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MangaUpdates Notifications.exe")}\"";
            StartupInfo info = new();
            RegistryKey? key;
            try
            {
                key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (key == null)
                {
                    Logger.Error("Failed to open the registry key.");
                    return info;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An error occured while openning the registry key.");
                return info;
            }

            string? keyValue;
            try
            {
                keyValue = key.GetValue(name)?.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An error occured while trying to get the registry value.");
                return info;
            }
            if (!string.IsNullOrEmpty(keyValue))
            {
                info.InStartup = keyValue.StartsWith(value);
                info.Portable = keyValue.Contains("-p");
                info.Minimized = keyValue.Contains("-m");
            }

            return info;
        }
    }
}