using System.Collections.Generic;
using Newtonsoft.Json;

namespace MangaUpdates_Notifications.Models
{
    public class Author
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public Image Image { get; set; } = new();

        [JsonProperty("actualname")]
        public string ActualName { get; set; } = string.Empty;

        public Birthday Birthday { get; set; } = new();
        public string BirthPlace { get; set; } = string.Empty;
        public string BloodType { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public List<string> Genres { get; set; } = new();
        public Stats Stats { get; set; } = new();
        public Social Social { get; set; } = new();

        [JsonProperty("comments")]
        public string Notes { get; set; } = string.Empty;

        [JsonProperty("last_updated")]
        public LastUpdated LastUpdated { get; set; } = new();

        public string GenresText => string.Join(", ", Genres);

        public string author_id { set { Id = value; } }
        public string Type { get; set; } = string.Empty;
        public string DisplayName => $"{Name} ({Type})";
    }
}