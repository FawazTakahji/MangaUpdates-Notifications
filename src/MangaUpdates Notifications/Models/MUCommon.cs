using Newtonsoft.Json;
using System;

namespace MangaUpdates_Notifications.Models
{
    public class Url
    {
        public string Original { get; set; } = string.Empty;
    }

    public class Image
    {
        public Url Url { get; set; } = new();
    }

    public class Anime
    {
        public string Start { get; set; } = string.Empty;
        public string End { get; set; } = string.Empty;
    }

    public class RelatedSeries
    {
        [JsonProperty("related_series_id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("related_series_name")]
        public string Title { get; set; } = string.Empty;
    }

    public class Recommendation
    {
        [JsonProperty("series_name")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("series_id")]
        public string Id { get; set; } = string.Empty;
    }

    public class LastUpdated
    {
        public long Timestamp { get; set; }
        public string String => DateTimeOffset.FromUnixTimeSeconds(Timestamp).DateTime.ToString("dd/MM/yyyy");
    }

    public class Birthday
    {
        [JsonProperty("as_string")]
        public string String { get; set; } = string.Empty;

        public string Zodiac { get; set; } = string.Empty;
    }

    public class Social
    {
        public string OfficialSite { get; set; } = string.Empty;
        public string Facebook { get; set; } = string.Empty;
        public string Twitter { get; set; } = string.Empty;
    }

    public class Stats
    {
        [JsonProperty("total_series")]
        public long TotalSeries { get; set; }

        [JsonProperty("total_publications")]
        public long TotalPublications { get; set; }
    }
}