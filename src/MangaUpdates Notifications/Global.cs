using MangaUpdates_Notifications.ViewModels;
using MangaUpdates_Notifications.Views;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Windows;
using MangaUpdates_Notifications.Extensions;
using MangaUpdates_Notifications.Utilities;
using Microsoft.Toolkit.Uwp.Notifications;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace MangaUpdates_Notifications
{
    public static class Global
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static readonly SemaphoreSlim DialogSemaphore = new(1, 1);
        public static string BaseDir { get; private set; }
        public static Models.Settings Settings { get; set; }
        public static HttpClient HttpClient { get; } = new();
        public static LibraryViewModel Library { get; private set; }
        public static NotificationsViewModel Notifications { get; private set; }
        public static MainWindowViewModel MainViewModel { get; private set; }
        public static MainWindowView MainView { get; private set; }
        private static Mutex _mutex = null;

        public static void Initialize(string[] args)
        {
            DialogSemaphore.Wait();
            SetBaseDir(args);
            SetMutex();
            SetupNLog();

            Version version = typeof(Global).Assembly.GetName().Version.GetMajorMinorBuild();
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"MangaUpdates Notifications/{version}");

            Library = new();
            Notifications = new();
            MainViewModel = new();
            MainView = new() { DataContext = MainViewModel };

            WindowStartup(args);
            DialogSemaphore.Release();
            SubscribeToNotifications();
        }

        private static void SetBaseDir(string[] args)
        {
            if (args.Contains("--portable", StringComparer.OrdinalIgnoreCase) || args.Contains("-p", StringComparer.OrdinalIgnoreCase))
                if (new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).IsAccessible())
                    BaseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                else
                {
                    BaseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MangaUpdates Notifications");
                    _ = Misc.ShowDialog($"Failed to use portable mode because the app folder can't be accessed.\nThe following folder will be used instead:\n{BaseDir}");
                }
            else
                BaseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MangaUpdates Notifications");
            Directory.CreateDirectory(BaseDir);
        }

        private static void SetMutex()
        {
            string mutex = BaseDir.Replace(Path.DirectorySeparatorChar, char.Parse("/"));
            _mutex = new(false, @"Global\" + mutex);

            if (!_mutex.WaitOne(0, false))
            {
                MessageBox.Show("Another instance is already using the current folder.",
                    "MangaUpdates Notifications", MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Shutdown();
            }
        }

        private static void SetupNLog()
        {
            LoggingConfiguration config = new();
            FileTarget fileTarget = new("file") { FileName = $@"{BaseDir}/Logs/${{date:format=yyyy-MM-dd HH\:mm\:ss:cached=true}}.log" };
            config.AddTarget(fileTarget);
            config.AddRule(new LoggingRule("*", LogLevel.Info, fileTarget));
            LogManager.Configuration = config;

            Logger.Info($"The base directory is \"{BaseDir}\".");
        }

        private static void WindowStartup(string[] args)
        {
            MainView.TrayIcon.ForceCreate(false);
            Utilities.Settings.ApplySettings(Utilities.Settings.LoadSettings(Path.Combine(BaseDir, "Settings.json")));
            if (!args.Contains("--minimize", StringComparer.OrdinalIgnoreCase) && !args.Contains("-m", StringComparer.OrdinalIgnoreCase))
                MainView.Show();
        }

        private static void SubscribeToNotifications()
        {
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                ToastArguments args = ToastArguments.Parse(toastArgs.Argument);
                Application.Current.Dispatcher.Invoke(() => // Run on main thread.
                {
                    switch (args["action"])
                    {
                        case "read":
                        {
                            _ = Notifications.RemoveNotification(args["title"], args["chapter"], args["scanlator"]);
                            break;
                        }
                        case "view":
                        {
                            MainViewModel.Navigate("Notifications");
                            if (MainViewModel.FrameContent is NotificationsView currentView)
                                currentView.ScrollToNotification(args["title"], args["chapter"], args["scanlator"]);
                            MainView.Show();
                            break;
                        }
                    }
                });
            };
        }
    }
}