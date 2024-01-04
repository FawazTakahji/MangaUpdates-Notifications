using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MangaUpdates_Notifications.Utilities
{
    internal static class PushBullet
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static async Task Notify(string title, string url, string chapter, string scanlators, string deviceId = "")
        {
            if (string.IsNullOrEmpty(Global.Settings.PushBullet.Token))
            {
                Logger.Error("PushBullet token is empty.");
                return;
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.pushbullet.com/v2/pushes");
            var body = new
            {
                type = "link",
                url,
                title,
                body = $"Chapter: {chapter}\nScanlator: {scanlators}",
                device_iden = deviceId
            };
            request.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body),Encoding.UTF8, "application/json");
            request.Headers.Add("Access-Token", Global.Settings.PushBullet.Token);
            try
            {
                using var response = await Global.HttpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    string message = !string.IsNullOrEmpty(deviceId)
                        ? $"Failed to send a message to device \"{deviceId}\".\nStatus Code: \"{response.StatusCode}\" - Reason Phrase: \"{response.ReasonPhrase}\"."
                        : $"Failed to send a message to PushBullet devices.\nStatus Code: \"{response.StatusCode}\" - Reason Phrase: \"{response.ReasonPhrase}\".";
                    _ = Misc.ShowDialog(message);
                    Logger.Error(message);
                }
            }
            catch (Exception ex)
            {
                string message = !string.IsNullOrEmpty(deviceId)
                    ? $"Failed to send a message to device \"{deviceId}\"."
                    : "Failed to send a message to PushBullet devices.";
                _ = Misc.ShowDialog(message, ex);
                Logger.Error(ex, message);
            }
        }
    }
}