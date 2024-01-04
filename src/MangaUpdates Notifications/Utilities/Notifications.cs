using MangaUpdates_Notifications.Models;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MangaUpdates_Notifications.Utilities
{
    internal static class Notifications
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static readonly System.Timers.Timer CheckTimer;

        static Notifications()
        {
            CheckTimer = new System.Timers.Timer(TimeSpan.FromHours(1));
            CheckTimer.Elapsed += (_, _) => _ = CheckReleases();
            CheckTimer.Start();
        }

        public static async Task CheckReleases()
        {
            if (Global.Library.Mangas.Count == 0)
                return;

            await Global.Library.CollectionAccess.WaitAsync();
            ObservableCollection<Manga> mangas = Global.Library.Mangas;
            Global.Library.CollectionAccess.Release();

            foreach (var manga in mangas)
            {
                if (manga.Completed)
                {
                    Logger.Info($"Didn't check the releases for the manga \"{manga.Id}\" because its marked as completed.");
                    return;
                }

                List<Manga.Release> releases = await MangaUpdates.GetSeriesNewReleases(
                    manga.Id, manga.LatestRelease, manga.PreferredScanlator, manga.AnyScanlator);
                if (releases.Count == 0)
                    return;

                await Global.Library.CollectionAccess.WaitAsync();
                Manga originalItem = Global.Library.Mangas.FirstOrDefault(item => item.Id ==  manga.Id);
                if (originalItem != null)
                    originalItem.LatestRelease = releases.FirstOrDefault();
                Global.Library.CollectionAccess.Release();

                for (int i = releases.Count - 1; i >= 0; i--)
                {
                    Manga.Release release = releases[i];
                    string chapter = Regex.Match(release.Title, @"c\.(.+)$").Groups[1].Value;
                    _ = Global.Notifications.AddNotification(manga.Title, manga.Image.Url.Original, chapter, release.Scanlator);

                    if(Global.Settings.Personalization.WindowsNotifications)
                        NotifyWindows(manga.Title, chapter, release.Scanlator);

                    if (Global.Settings.DiscordWebhook.Enabled || Global.Settings.DiscordBot.Enabled)
                    {
                        string scanlators = await MangaUpdates.GetScanlatorsUrls(release.Scanlator);
                        StringContent content = Discord.GenerateDiscordContent(manga.Title, manga.Url, manga.Image.Url.Original, chapter, scanlators);

                        if (Global.Settings.DiscordWebhook.Enabled)
                        {
                            foreach (var webhook in Global.Settings.DiscordWebhook.Webhooks)
                            {
                                await Discord.DiscordWebhook(content, webhook.String, manga.Title, chapter);
                            }
                        }

                        if (Global.Settings.DiscordBot.Enabled)
                        {
                            foreach (var user in Global.Settings.DiscordBot.Users)
                            {
                                await Discord.DiscordUser(content, user.String, manga.Title, chapter);
                            }

                            foreach (var channel in Global.Settings.DiscordBot.Channels)
                            {
                                await Discord.DiscordChannel(content, channel.String, manga.Title, chapter);
                            }
                        }
                    }

                    if (Global.Settings.PushBullet.Enabled)
                    {
                        if (Global.Settings.PushBullet.AllDevices)
                            await PushBullet.Notify(manga.Title, manga.Url, chapter, release.Scanlator);
                        else
                        {
                            foreach (var device in Global.Settings.PushBullet.Devices)
                            {
                                await PushBullet.Notify(manga.Title, manga.Url, chapter, release.Scanlator, device.String);
                            }
                        }
                    }
                }
            }

            Global.Library.SaveLibrary();
        }

        private static void NotifyWindows(string title, string chapter, string scanlator)
        {
            new ToastContentBuilder()
            .AddArgument("action", "view") // Action to be performed when the notification is clicked.
            .AddArgument("title", title) .AddArgument("chapter", chapter) .AddArgument("scanlator", scanlator)
            .AddText(title)
            .AddVisualChild(new AdaptiveGroup()
            {
                Children =
                {
                    new AdaptiveSubgroup()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "Chapter",
                                HintStyle = AdaptiveTextStyle.Base
                            },
                            new AdaptiveText()
                            {
                                Text = chapter,
                                HintStyle = AdaptiveTextStyle.CaptionSubtle
                            }
                        }
                    },
                    new AdaptiveSubgroup()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "Scanlator",
                                HintStyle = AdaptiveTextStyle.Base,
                            },
                            new AdaptiveText()
                            {
                                Text = scanlator,
                                HintStyle = AdaptiveTextStyle.CaptionSubtle,
                            }
                        }
                    }
                }
            })

            .AddButton(new ToastButton()
             .SetContent("Mark As Read")
             .AddArgument("action", "read") // Action to be performed when the button is clicked.
             .SetBackgroundActivation())

            .Show();

            if (!Global.MainView.IsActive && Global.MainView.IsVisible)
                WindowFlasher.FlashUntilActivated(Global.MainView);
        }

        public static ObservableCollection<Notification> LoadNotifications(string path)
        {
            ObservableCollection<Notification> notifications = new();
            if (!File.Exists(path))
            {
                Logger.Warn("The notifications file doesn't exist.");
                return notifications;
            }

            try
            {
                string json = File.ReadAllText(path);
                if (json.Length > 2)
                    notifications = JsonConvert.DeserializeObject<ObservableCollection<Notification>>(json) ?? notifications;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An error occured while loading the notifications.");
            }

            return notifications;
        }

        public static void SaveNotifications(string path, ObservableCollection<Notification> collection)
        {
            try
            {
                string json = JsonConvert.SerializeObject(collection,
                    new JsonSerializerSettings { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore });
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An error occurred while saving the notifications.");
            }
        }
    }
}
