using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MangaUpdates_Notifications.Utilities
{
    internal static class Discord
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static async Task DiscordWebhook(StringContent content, string webhook, string mangaTitle, string chapter)
        {
            if (string.IsNullOrEmpty(webhook))
            {
                Logger.Error("Webhook is empty.");
                return;
            }

            try
            {
                using var response = await Global.HttpClient.PostAsync(webhook, content);
                if (!response.IsSuccessStatusCode)
                {
                    string message = $"Failed to send a notification for \"{mangaTitle}\" Chapter: {chapter}.\nStatus Code: \"{response.StatusCode}\" - Reason Phrase: \"{response.ReasonPhrase}\".";
                    _ = Misc.ShowDialog(message);
                    Logger.Error(message);
                }
            }
            catch (Exception ex)
            {
                string message = $"Failed to send a notification for \"{mangaTitle}\" Chapter: {chapter}.";
                _ = Misc.ShowDialog(message, ex);
                Logger.Error(ex, message);
            }
        }

        public static async Task DiscordUser(StringContent content, string user, string mangaTitle, string chapter)
        {
            if (string.IsNullOrEmpty(Global.Settings.DiscordBot.Token))
            {
                Logger.Error("Discord bot token is empty.");
                return;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, "https://discord.com/api/v10/users/@me/channels");
            request.Content = new StringContent($"{{ \"recipient_id\": \"{user}\" }}", Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", $"Bot {Global.Settings.DiscordBot.Token}");

            try
            {
                using var response = await Global.HttpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    string message = $"Failed to create a DM channel with user \"{user}\".\nStatus Code: \"{response.StatusCode}\" - Reason Phrase: \"{response.ReasonPhrase}\".";
                    _ = Misc.ShowDialog(message);
                    Logger.Error(message);
                    return;
                }

                string responseContent = await response.Content.ReadAsStringAsync();
                string channelId = JObject.Parse(responseContent)["id"].ToString();

                if (!string.IsNullOrEmpty(channelId))
                    await DiscordChannel(content, channelId, mangaTitle, chapter);
                else
                    Logger.Error("Channel id is empty.");
            }
            catch (Exception ex)
            {
                string message = $"Failed to send a message to user \"{user}\", for \"{mangaTitle}\" Chapter: {chapter}.";
                _ = Misc.ShowDialog(message, ex);
                Logger.Error(ex, message);
            }
        }

        public static async Task DiscordChannel(StringContent content, string channel, string mangaTitle, string chapter)
        {
            if (string.IsNullOrEmpty(Global.Settings.DiscordBot.Token))
            {
                Logger.Error("Discord bot token is empty.");
                return;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://discord.com/api/v10/channels/{channel}/messages");
            request.Content = content;
            request.Headers.Add("Authorization", $"Bot {Global.Settings.DiscordBot.Token}");

            try
            {
                using var response = await Global.HttpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    string message = $"Failed to send a message to channel \"{channel}\", for \"{mangaTitle}\" Chapter: {chapter}.\nStatus Code: \"{response.StatusCode}\" - Reason Phrase: \"{response.ReasonPhrase}\".";
                    _ = Misc.ShowDialog(message);
                    Logger.Error(message);
                }
            }
            catch (Exception ex)
            {
                string message = $"Failed to send a message to channel \"{channel}\".";
                _ = Misc.ShowDialog(message, ex);
                Logger.Error(ex, message);
            }
        }

        public static StringContent GenerateDiscordContent(string title, string url, string imageUrl, string chapter, string scanlator)
        {
            var embed = new
            {
                title,
                url,
                description = "A new chapter has been released.",
                color = 16744515,
                fields = new[]
                {
                    new { name = "Chapter", value = chapter, inline = true },
                    new { name = "Scanlator", value = scanlator, inline = true }
                },
                image = new { url = imageUrl }
            };

            return new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new { embeds = new[] { embed } }), Encoding.UTF8, "application/json");
        }
    }
}