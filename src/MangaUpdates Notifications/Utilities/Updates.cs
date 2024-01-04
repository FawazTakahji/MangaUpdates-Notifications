using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using MangaUpdates_Notifications.Extensions;
using ModernWpf.Controls;
using Newtonsoft.Json.Linq;

namespace MangaUpdates_Notifications.Utilities
{
    internal static class Updates
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static async Task Check()
        {
            Version currentVersion = typeof(Updates).Assembly.GetName().Version.GetMajorMinorBuild();
            Version? githubVersion = await GetLatestVersion();

            if (githubVersion?.CompareTo(currentVersion) > 0)
            {
                await Global.DialogSemaphore.WaitAsync();
                ContentDialog dialog = new()
                {
                    Title = "Update Available",
                    Content = $"A newer version of the app is available (v{githubVersion})\nWould you like to upgrade?",
                    SecondaryButtonText = "Yes",
                    PrimaryButtonText = "No",
                    SecondaryButtonStyle = (Style)Application.Current.FindResource("AccentButtonStyle")
                };
                dialog.SecondaryButtonClick += (_, _) =>
                {
                    Process.Start(new ProcessStartInfo("https://github.com/FawazTakhji/MangaUpdates-Notifications/releases/latest") { UseShellExecute = true });
                };
                await dialog.ShowAsync();
                Global.DialogSemaphore.Release();
            }
        }

        private static async Task<Version?> GetLatestVersion()
        {
            Version? latestVersion = null;

            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/repos/FawazTakhji/MangaUpdates-Notifications/releases/latest");
            request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");

            try
            {
                using var response = await Global.HttpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    JObject responseObj = JObject.Parse(responseContent);
                    if (responseObj.TryGetValue("tag_name", out var value))
                        latestVersion = Version.Parse(value.ToString());
                    else
                    {
                        string message = "Failed to get a valid version number from the GitHub api.";
                        Logger.Warn(message);
                        _ = Misc.ShowDialog(message);
                    }
                }
                else
                {
                    string message = $"Failed to get a valid version number from the GitHub api.\nStatus Code: \"{response.StatusCode}\" - Reason Phrase: \"{response.ReasonPhrase}\".";
                    Logger.Warn(message);
                    _ = Misc.ShowDialog(message);
                }
            }
            catch (Exception ex)
            {
                string message = "Failed to get the latest github version.";
                Logger.Warn(ex, message);
                _ = Misc.ShowDialog(message, ex);
            }

            return latestVersion;
        }
    }
}