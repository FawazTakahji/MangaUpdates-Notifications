using System;

namespace MangaUpdates_Notifications.Extensions
{
    public static class VersionExtensions
    {
        public static Version GetMajorMinorBuild(this Version version)
        {
            return new Version(version.Major, version.Minor, version.Build);
        }
    }
}