using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using MangaUpdates_Notifications.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MangaUpdates_Notifications.Utilities
{
    internal static class MangaUpdates
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly MemoryCache ScanlatorCache = MemoryCache.Default;

        public static async Task<Search> Search(int page, string searchQuery, Filters filters)
        {
            Search search = new();

            List<string> filteredTypes = new(); // types added to this list won't show up in the search results
            foreach (var property in filters.GetType().GetProperties())
            {
                if (property.PropertyType == typeof(bool) && (bool)property.GetValue(filters) == false)
                    filteredTypes.Add(property.Name);
            }
            var body = new
            {
                search = searchQuery,
                page,
                filter_types = filteredTypes.ToArray()
            };
            using var content = new StringContent(JsonConvert.SerializeObject(body), System.Text.Encoding.UTF8, "application/json");

            try
            {
                using var response = await Global.HttpClient.PostAsync("https://api.mangaupdates.com/v1/series/search", content);
                if (!response.IsSuccessStatusCode)
                {
                    string message = $"Failed to get the search results.\nStatus Code: \"{response.StatusCode}\" - Reason Phrase: \"{response.ReasonPhrase}\".";
                    Logger.Error(message);
                    _ = Misc.ShowDialog(message);
                    return search;
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                search = JsonConvert.DeserializeObject<Search>(jsonResponse) ?? search;
                foreach (var result in search.Results)
                {
                    result.Record.Title = WebUtility.HtmlDecode(result.Record.Title);
                }
            }
            catch (Exception ex)
            {
                string message = "Failed to get the search results.";
                Logger.Error(ex, message);
                _ = Misc.ShowDialog(message, ex);
            }

            return search;
        }

        public static async Task<MangaInfo> GetSeries(string id)
        {
            string url = $"https://api.mangaupdates.com/v1/series/{id}?unrenderedFields=true";
            MangaInfo series = new();

            try
            {
                using var response = await Global.HttpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Error($"Failed to get the data for the manga \"{id}\".\nStatus Code: \"{response.StatusCode}\" - Reason Phrase: \"{response.ReasonPhrase}\".");
                    return series;
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                series = JsonConvert.DeserializeObject<MangaInfo>(jsonResponse) ?? series;
                if (!string.IsNullOrEmpty(series.Description))
                {
                    //remove tags
                    series.Description = Regex.Replace(series.Description, @"\[(\w+)](.*?)\[\/\1]", m => m.Groups[2].Value);
                    //remove hyperlinks
                    series.Description = Regex.Replace(series.Description, @"\[url=([^\]]+)](.*?)\[\/url]", m => m.Groups[2].Value);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to get the info for the manga \"{id}\".");
            }

            return series;
        }

        private static async Task<List<Manga.Release>> GetSeriesReleases(string id)
        {
            string url = $"https://api.mangaupdates.com/v1/series/{id}/rss";

            List<Manga.Release> releases = new();

            try
            {
                using var response = await Global.HttpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    string message = $"Failed to get the releases for the manga \"{id}\".\nStatus Code: \"{response.StatusCode}\" - Reason Phrase: \"{response.ReasonPhrase}\".";
                    _ = Misc.ShowDialog(message);
                    Logger.Error(message);
                    return releases;
                }

                string rssString = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(rssString))
                {
                    XDocument rss = XDocument.Parse(rssString);
                    IEnumerable<XElement> items = rss.Root.Element("channel").Elements("item").Where
                    (item =>
                        item.Element("title") != null && !string.IsNullOrEmpty(item.Element("title").Value) &&
                        item.Element("description") != null && !string.IsNullOrEmpty(item.Element("description").Value));
                    foreach (XElement item in items)
                    {
                        Manga.Release release = new Manga.Release
                        {
                            Title = item.Element("title").Value,
                            Scanlator = item.Element("description").Value
                        };
                        releases.Add(release);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = $"Failed to get the releases for the manga \"{id}\".";
                _ = Misc.ShowDialog(message, ex);
                Logger.Error(ex, message);
            }

            return releases;
        }

        public static async Task<Manga.Release> GetSeriesLatestRelease(string id)
        {
            List<Manga.Release> releases = await GetSeriesReleases(id);
            return releases.FirstOrDefault(new Manga.Release());
        }

        public static async Task<List<Manga.Release>> GetSeriesNewReleases(string id, Manga.Release release, string? scanlator, bool anyScanlator)
        {
            List<Manga.Release> releases = await GetSeriesReleases(id);
            if (releases.Count == 0)
            {
                Logger.Info($"Didn't find any new releases for the manga \"{id}\".");
                return releases;
            }

            if (anyScanlator || string.IsNullOrEmpty(scanlator))
                releases = releases.TakeWhile(item => item.Title != release.Title || item.Scanlator != release.Scanlator).ToList();
            else
            {
                List<Manga.Release> filteredReleases = releases.TakeWhile(item => item.Title != release.Title || item.Scanlator != release.Scanlator).ToList();
                releases = filteredReleases.Where(item => item.Scanlator.Split(',').Any(s => s.Trim() == scanlator)).ToList();
            }

            return releases;
        }

        public static async Task<List<string>> GetSeriesScanlators(string id)
        {
            List<string> scanlators = new();
            string url = $"https://api.mangaupdates.com/v1/series/{id}/groups";
            try
            {
                using var response = await Global.HttpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Error($"Failed to get the scanlators for the manga \"{id}\".");
                    return scanlators;
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(jsonResponse);
                if (jsonObject["group_list"] is JArray groupList)
                {
                    foreach (JObject group in groupList)
                    {
                        var isGroupActive = group["active"]?.Value<bool?>();
                        if (isGroupActive.HasValue && isGroupActive.Value)
                        {
                            var groupName = group["name"]?.Value<string>();
                            if (!string.IsNullOrEmpty(groupName))
                            {
                                groupName = WebUtility.HtmlDecode(groupName);
                                scanlators.Add(groupName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to get the scanlators for the manga \"{id}\".");
            }

            return scanlators;
        }

        public static async Task<Author> GetAuthor(string id)
        {
            string url = $"https://api.mangaupdates.com/v1/authors/{id}?unrenderedFields=true";
            Author author = new();

            try
            {
                using var response = await Global.HttpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    string message = $"Failed to get the info for the author \"{id}\".\nStatus Code: \"{response.StatusCode}\" - Reason Phrase: \"{response.ReasonPhrase}\".";
                    _ = Misc.ShowDialog(message);
                    Logger.Error(message);
                    return author;
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                author = JsonConvert.DeserializeObject<Author>(jsonResponse) ?? author;
                if (!string.IsNullOrEmpty(author.Notes))
                {
                    //remove tags
                    author.Notes = Regex.Replace(author.Notes, @"\[(\w+)](.*?)\[\/\1]", m => m.Groups[2].Value);
                    //remove hyperlinks
                    author.Notes = Regex.Replace(author.Notes, @"\[url=([^\]]+)](.*?)\[\/url]", m => m.Groups[2].Value);
                }
            }
            catch (Exception ex)
            {
                string message = $"Failed to get the info for the author \"{id}\".";
                Logger.Error(ex, message);
                _ = Misc.ShowDialog(message, ex);
            }

            return author;
        }

        public static async Task<Publisher> GetPublisher(string id)
        {
            string url = $"https://api.mangaupdates.com/v1/publishers/{id}?unrenderedFields=true";
            Publisher publisher = new();

            try
            {
                using var response = await Global.HttpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    string message = $"Failed to get the info for the publisher \"{id}\".\nStatus Code: \"{response.StatusCode}\" - Reason Phrase: \"{response.ReasonPhrase}\".";
                    _ = Misc.ShowDialog(message);
                    Logger.Error(message);
                    return publisher;
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                publisher = JsonConvert.DeserializeObject<Publisher>(jsonResponse) ?? publisher;
                if (!string.IsNullOrEmpty(publisher.Notes))
                {
                    //remove tags
                    publisher.Notes = Regex.Replace(publisher.Notes, @"\[(\w+)](.*?)\[\/\1]", m => m.Groups[2].Value);
                    //remove hyperlinks
                    publisher.Notes = Regex.Replace(publisher.Notes, @"\[url=([^\]]+)](.*?)\[\/url]", m => m.Groups[2].Value);
                }
            }
            catch (Exception ex)
            {
                string message = $"Failed to get the info for the publisher \"{id}\".";
                Logger.Error(ex, message);
                _ = Misc.ShowDialog(message, ex);
            }

            return publisher;
        }

        public static async Task<string> GetScanlatorsUrls(string scanlators)
        {
            string[] splitScanlators = scanlators.Split(',');
            for (int i = 0; i < splitScanlators.Length; i++)
            {
                string key = splitScanlators[i].Trim();
                if (ScanlatorCache.Contains(key))
                {
                    splitScanlators[i] = $"[{splitScanlators[i].Trim()}]({ScanlatorCache.Get(key)})";
                }
                else
                {
                    string scanlatorUrl = await GetScanlatorUrl(splitScanlators[i].Trim());
                    if (!string.IsNullOrEmpty(scanlatorUrl))
                    {
                        ScanlatorCache.Add(key, scanlatorUrl, new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddDays(1) });
                        splitScanlators[i] = $"[{splitScanlators[i].Trim()}]({scanlatorUrl})";
                    }
                }
            }

            return string.Join(", ", splitScanlators);
        }

        private static async Task<string> GetScanlatorUrl(string searchQuery)
        {
            StringContent content = new StringContent(
                JsonConvert.SerializeObject(new { search = searchQuery }), System.Text.Encoding.UTF8, "application/json");
            string url = string.Empty;

            try
            {
                using var response = await Global.HttpClient.PostAsync("https://api.mangaupdates.com/v1/groups/search", content);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Error($"Failed to get the url for the scanlator \"{searchQuery}\".");
                    return url;
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(jsonResponse);
                JToken scanlator = jsonObject["results"].FirstOrDefault(
                    item => item["record"]["name"].Value<string>() == searchQuery || item["hit_name"].Value<string>() == searchQuery);

                url = scanlator?["record"]?["url"]?.Value<string>() ?? string.Empty;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to get the url for the scanlator \"{searchQuery}\".");
            }

            return url;
        }
    }
}
