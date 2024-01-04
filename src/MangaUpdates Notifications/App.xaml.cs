using MangaUpdates_Notifications.Utilities;
using System.Windows;
using System.IO;
using System.Linq;

namespace MangaUpdates_Notifications
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Contains("--cleanup"))
                Cleanup.AppDataCleanup();

            base.OnStartup(e);
            Global.Initialize(e.Args);

            _ = Cache.DeleteOldFiles();
            Misc.DeleteOldFiles(Path.Combine(Global.BaseDir, "Logs"), 7);
            _ = Notifications.CheckReleases();

            if (Global.Settings.Personalization.CheckUpdates)
                _ = Updates.Check();
        }
    }
}