using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace MangaUpdates_Notifications.Utilities
{
    internal static class Cleanup
    {
        public static void AppDataCleanup()
        {
            RemoveDataFolder();
            RemoveAppDataFolder();
            Startup.RemoveFromStartup();
            Environment.Exit(Environment.ExitCode);
        }

        private static void RemoveDataFolder()
        {
            string dataFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(dataFolderPath))
                return;

            try
            {
                Directory.Delete(dataFolderPath, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to delete the data folder.{Environment.NewLine}{ex.Message}",
                    "Uninstall", MessageBoxButton.OK, MessageBoxImage.Error, 0, MessageBoxOptions.DefaultDesktopOnly);
                try
                {
                    Process.Start(new ProcessStartInfo(dataFolderPath + Path.DirectorySeparatorChar)
                        { UseShellExecute = true });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        private static void RemoveAppDataFolder()
        {
            string appdataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MangaUpdates Notifications");
            if (!Directory.Exists(appdataDirectory))
                return;

            MessageBoxResult result = MessageBox.Show(
                $"Do you want to delete the app data in {appdataDirectory}?",
                "Uninstall", MessageBoxButton.YesNo, MessageBoxImage.Question,
                MessageBoxResult.No, MessageBoxOptions.DefaultDesktopOnly);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                Directory.Delete(appdataDirectory, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete the app data.{Environment.NewLine}{ex.Message}",
                    "Uninstall", MessageBoxButton.OK, MessageBoxImage.Error, 0, MessageBoxOptions.DefaultDesktopOnly);
                try
                {
                    Process.Start(new ProcessStartInfo(appdataDirectory + Path.DirectorySeparatorChar) { UseShellExecute = true });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}