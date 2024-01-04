using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MangaUpdates_Notifications.Models;
using MangaUpdates_Notifications.Utilities;
using ModernWpf;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Settings = MangaUpdates_Notifications.Models.Settings;

namespace MangaUpdates_Notifications.ViewModels
{
    partial class SettingsViewModel : ObservableObject
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string SettingsPath = Path.Combine(Global.BaseDir, "Settings.json");
        [ObservableProperty] private Settings _appSettings;
        [ObservableProperty] private StartupInfo _startup;

        [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(SaveAndApplySettingsCommand))] [NotifyCanExecuteChangedFor(nameof(RestoreSettingsCommand))]
        private bool _isSettingsChanged;

        public SettingsViewModel()
        {
            AppSettings = Utilities.Settings.LoadSettings(SettingsPath);
            AppSettings.SettingsChanged += (_, _) => IsSettingsChanged = true;

            Startup = Utilities.Startup.GetStartupInfo();
            Startup.PropertyChanged += (_, _) => IsSettingsChanged = true;
        }

        [RelayCommand(CanExecute = nameof(IsSettingsChanged))]
        private void SaveAndApplySettings()
        {
            IsSettingsChanged = false;
            Utilities.Settings.SaveSettings(SettingsPath, AppSettings);
            Utilities.Settings.ApplySettings(AppSettings);
            if (Startup.InStartup)
                Utilities.Startup.AddToStartup(Startup.Portable, Startup.Minimized);
            else
                Utilities.Startup.RemoveFromStartup();
        }

        [RelayCommand(CanExecute = nameof(IsSettingsChanged))]
        private void RestoreSettings()
        {
            IsSettingsChanged = false;
            AppSettings = Utilities.Settings.LoadSettings(SettingsPath);
            Utilities.Settings.ApplySettings(AppSettings);
            AppSettings.SettingsChanged += (_, _) => IsSettingsChanged = true;

            Startup = Utilities.Startup.GetStartupInfo();
            Startup.PropertyChanged += (_, _) => IsSettingsChanged = true;
        }

        [RelayCommand] private void ResetSettings()
        {
            IsSettingsChanged = true;
            AppSettings = new();
            ThemeManager.Current.ApplicationTheme = AppSettings.Personalization.Theme;
            ThemeManager.Current.AccentColor = AppSettings.Personalization.Accent;
            Startup = new();
            Startup.PropertyChanged += (_, _) => IsSettingsChanged = true;
        }

        [RelayCommand] private static async Task TestWebhook(string webhook)
        {
            StringContent content = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(new { content = "This is a test message." }), Encoding.UTF8, "application/json");
            try
            {
                using var response = await Global.HttpClient.PostAsync(webhook, content);
                if (!response.IsSuccessStatusCode)
                {
                    string message = $"Failed to send a message to webhook \"{webhook}\".\nStatus Code: \"{response.StatusCode}\" - Reason Phrase: \"{response.ReasonPhrase}\".";
                    Logger.Error(message);
                    _ = Misc.ShowDialog(message);
                }
            }
            catch (Exception ex)
            {
                string message = $"Failed to send a message to webhook \"{webhook}\".";
                Logger.Error(ex, message);
                _ = Misc.ShowDialog(message);
            }
        }

        [RelayCommand] private async Task TestUser(string user)
        {
            if (string.IsNullOrEmpty(AppSettings.DiscordBot.Token))
            {
                _ = Misc.ShowDialog("Token can't be empty.");
                return;
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://discord.com/api/v10/users/@me/channels");
            request.Content = new StringContent($"{{ \"recipient_id\": \"{user}\" }}", Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", $"Bot {AppSettings.DiscordBot.Token}");
            try
            {
                using var response = await Global.HttpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    string message = $"Failed to create a DM channel with user \"{user}\".\nStatus Code: \"{response.StatusCode}\" - Reason Phrase: \"{response.ReasonPhrase}\".";
                    Logger.Error(message);
                    _ = Misc.ShowDialog(message);
                    return;
                }

                String responseContent = await response.Content.ReadAsStringAsync();
                String channelId = JObject.Parse(responseContent)["id"].ToString();
                _ = TestChannel(channelId);
            }
            catch (Exception ex)
            {
                string message = $"Failed to send a message to user \"{user}\".";
                Logger.Error(ex, message);
                _ = Misc.ShowDialog(message, ex);
            }
        }

        [RelayCommand] private async Task TestChannel(string channel)
        {
            if (string.IsNullOrEmpty(AppSettings.DiscordBot.Token))
            {
                _ = Misc.ShowDialog("Token can't be empty.");
                return;
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, $"https://discord.com/api/v10/channels/{channel}/messages");
            request.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new { content = "This is a test message." }), Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", $"Bot {AppSettings.DiscordBot.Token}");
            try
            {
                using var response = await Global.HttpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    string message = $"Failed to send a message to channel \"{channel}\".\nStatus Code: \"{response.StatusCode}\" - Reason Phrase: \"{response.ReasonPhrase}\".";
                    Logger.Error(message);
                    _ = Misc.ShowDialog(message);
                }
            }
            catch (Exception ex)
            {
                string message = $"Failed to send a message to channel \"{channel}\".";
                Logger.Error(ex, message);
                _ = Misc.ShowDialog(message, ex);
            }
        }

        [RelayCommand] private async Task TestPushBulletDevice(string deviceId = "")
        {
            if (string.IsNullOrEmpty(AppSettings.PushBullet.Token))
            {
                _ = Misc.ShowDialog("Token can't be empty.");
                return;
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.pushbullet.com/v2/pushes");
            string device_iden = !string.IsNullOrEmpty(deviceId) ? deviceId : null;
            var body = new
            {
                type = "note",
                title = "Test",
                body = "This is a test message.",
                device_iden
            };
            request.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body),Encoding.UTF8, "application/json");
            request.Headers.Add("Access-Token", AppSettings.PushBullet.Token);
            try
            {
                using var response = await Global.HttpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    string message = !string.IsNullOrEmpty(deviceId)
                        ? $"Failed to send a message to device \"{deviceId}\".\nStatus Code: \"{response.StatusCode}\" - Reason Phrase: \"{response.ReasonPhrase}\"."
                        : $"Failed to send a message to PushBullet devices.\nStatus Code: \"{response.StatusCode}\" - Reason Phrase: \"{response.ReasonPhrase}\".";
                    Logger.Error(message);
                    _ = Misc.ShowDialog(message);
                }
            }
            catch (Exception ex)
            {
                string message = !string.IsNullOrEmpty(deviceId)
                    ? $"Failed to send a message to device \"{deviceId}\"."
                    : "Failed to send a message to PushBullet devices.";
                Logger.Error(ex, message);
                _ = Misc.ShowDialog(message, ex);
            }
        }
    }
}