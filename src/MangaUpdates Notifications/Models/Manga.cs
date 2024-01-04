using System;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace MangaUpdates_Notifications.Models
{
    public partial class Manga : ObservableObject
    {
        [ObservableProperty] [property: JsonProperty(Order = -2)]
        private string _title = string.Empty;

        [ObservableProperty] private Image _image = new();

        [JsonProperty("series_id")]
        public string Id { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;
        public bool Completed { get; set; }
        public string PreferredScanlator { get; set; } = string.Empty;
        public bool AnyScanlator { get; set; }
        public Release LatestRelease { get; set; } = new();
        public DateTime LastUpdated { get; set; }
        public partial class Release : ObservableObject
        {
            [NotifyPropertyChangedFor(nameof(DisplayTitle))] [ObservableProperty]
            private string _title = string.Empty;

            [ObservableProperty] private string _scanlator = string.Empty;

            [JsonIgnore]
            public string DisplayTitle => "Chapter " + Regex.Match(Title, @"c\.(.+)$").Groups[1].Value;
        }
    }
}