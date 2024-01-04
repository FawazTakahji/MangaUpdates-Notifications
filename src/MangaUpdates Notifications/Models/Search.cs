using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace MangaUpdates_Notifications.Models
{
    public partial class Search : ObservableObject
    {
        [JsonProperty("total_hits")]
        public int TotalHits { get; set; }

        [JsonProperty("page")]
        public int CurrentPage { get; set; }

        [JsonProperty("per_page")]
        public int PerPage { get; set; }

        [ObservableProperty] private List<Result> _results = new();

        public int TotalPages => TotalHits > 0 && PerPage > 0 ? (int)Math.Ceiling((decimal)TotalHits / PerPage) : 0;

        public List<int> Pages
        {
            get
            {
                List<int> pageRange = new();
                if (TotalPages > 0)
                {
                    int start = Math.Max(1, CurrentPage - 3);
                    int end = Math.Min(TotalPages, CurrentPage + 3);

                    if (end - start < 6)
                    {
                        int diff = 6 - (end - start);
                        if (CurrentPage - start <= end - CurrentPage)
                            end = Math.Min(TotalPages, end + diff);
                        else
                            start = Math.Max(1, start - diff);
                    }

                    for (int i = start; i <= end; i++)
                    {
                        pageRange.Add(i);
                    }
                }
                return pageRange;
            }
        }

        public class Result
        {
            public Manga Record { get; set; } = new();
        }
    }
}
