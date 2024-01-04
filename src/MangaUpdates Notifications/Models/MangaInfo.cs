using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MangaUpdates_Notifications.Models
{


    public class MangaInfo
    {
        [JsonProperty("series_id")]
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Image Image { get; set; } = new();
        public string Type { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;

        [JsonProperty("bayesian_rating")]
        public string Rating { get; set; } = string.Empty;

        [JsonProperty("rating_votes")]
        public string Votes { get; set; } = string.Empty;

        public List<StringWrapper> Genres { get; set; } = new();
        public List<StringWrapper> Categories { get; set; } = new();

        [JsonProperty("latest_chapter")]
        public string LatestChapter { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
        public bool Licensed { get; set; }
        public bool Completed { get; set; }
        public Anime Anime { get; set; } = new();

        [JsonProperty("related_series")]
        public List<RelatedSeries> RelatedSeries { get; set; } = new();

        public List<Author> Authors { get; set; } = new();
        public List<Publisher> Publishers { get; set; } = new();

        [JsonProperty("category_recommendations")]
        public List<Recommendation> Recommendations { get; set; } = new();

        [JsonProperty("last_updated")]
        public LastUpdated LastUpdated { get; set; } = new();

        public string GenresText => string.Join(" - ", Genres.Select(wrapper => wrapper.String));
        public string CategoriesText => string.Join(" - ", Categories.Select(wrapper => wrapper.String));
        public string LicensedText => Licensed ? "Licensed in English? Yes" : "Licensed in English? No";
        public string CompletedText => Completed ? "Completely Scanlated? Yes" : "Completely Scanlated? No";
        public string Info
        {
            get
            {
                string infoString = string.Empty;;
                if (!string.IsNullOrEmpty(Rating))
                    infoString = $"Rating: {Rating} / 10";
                if (!string.IsNullOrEmpty(Votes))
                    infoString += infoString.Contains("Rating:") ? $" | {Votes} votes" : $"{Votes} votes";
                if (!string.IsNullOrEmpty(Type))
                    infoString += $"\nType: {Type}";
                if (!string.IsNullOrEmpty(Year))
                    infoString += infoString.Contains("Type:") ? $" | Year: {Year}" : $"\nYear: {Year}";
                if (!string.IsNullOrEmpty(LatestChapter))
                    infoString += $"\nLatest Chapter: {LatestChapter}";
                if (!string.IsNullOrEmpty(LastUpdated.String))
                    infoString += $"\nLast Updated: {LastUpdated.String}";
                return infoString;
            }
        }
    }
}
