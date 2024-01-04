using Newtonsoft.Json;

namespace MangaUpdates_Notifications.Models
{
    public class Publisher
    {
        public string Name { get; set; } = string.Empty;

        [JsonProperty("publisher_id")]
        public string Id { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;

        [JsonProperty("info")]
        public string Notes { get; set; } = string.Empty;

        public string Site { get; set; } = string.Empty;
        public Stats Stats { get; set; } = new();

        [JsonProperty("last_updated")]
        public LastUpdated LastUpdated { get; set; } = new();

        public string publisher_name { set { Name = value; } }
        public string DisplayName => $"{Name} ({Type})";
    }
}