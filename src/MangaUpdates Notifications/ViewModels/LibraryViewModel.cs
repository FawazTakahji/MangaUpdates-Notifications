using CommunityToolkit.Mvvm.ComponentModel;
using MangaUpdates_Notifications.Models;
using MangaUpdates_Notifications.Utilities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;
using MangaUpdates_Notifications.Enums;

namespace MangaUpdates_Notifications.ViewModels
{
    public partial class LibraryViewModel : ObservableObject
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        [ObservableProperty] private string _filePath = Path.Combine(Global.BaseDir, "Library.json");
        private readonly DispatcherTimer _infoTimer;
        public readonly SemaphoreSlim CollectionAccess = new(1, 1);
        [ObservableProperty] private ObservableCollection<Manga> _mangas;
        [ObservableProperty] private ICollectionView _mangasView;
        [ObservableProperty] private string _filterText = string.Empty;

        public LibraryViewModel()
        {
            _infoTimer = new DispatcherTimer();
            _infoTimer.Interval = TimeSpan.FromHours(12);
            _infoTimer.Tick += CheckManga;
            _infoTimer.Start();

            Mangas = Library.LoadLibrary(FilePath);
            Mangas.CollectionChanged += (_, _) => SaveLibrary();

            MangasView = CollectionViewSource.GetDefaultView(Mangas);
            MangasView.Filter = FilterMangas;
            MangasView.SortDescriptions.Add(new SortDescription(nameof(Manga.Title), ListSortDirection.Ascending));
        }

        private bool FilterMangas(object obj)
        {
            if (obj is Manga manga)
            {
                return (manga.Title.Contains(FilterText, StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }

        partial void OnFilterTextChanged(string value)
        {
            MangasView.Refresh();
        }

        partial void OnFilePathChanged(string? oldValue, string newValue)
        {
            if (oldValue != null && newValue != oldValue)
            {
                Mangas = Library.LoadLibrary(newValue);
            }
        }

        public async void AddEditManga(string title, string id, string url, string imageUrl, bool inLibrary)
        {
            Controls.MangaDialog dialog = new Controls.MangaDialog
            {
                Title = title,
                MangaId = id,
                InLibrary = inLibrary
            };

            if (inLibrary)
            {
                await CollectionAccess.WaitAsync();
                Manga manga = Mangas.FirstOrDefault(item => item.Id == id);
                if (manga.AnyScanlator)
                    dialog.AnyScanlator = true;
                else
                {
                    dialog.AnyScanlator = false;
                    dialog.SelectedScanlator = manga.PreferredScanlator;
                }
                CollectionAccess.Release();
            }

            await Global.DialogSemaphore.WaitAsync();
            await dialog.ShowAsync();
            Global.DialogSemaphore.Release();

            await CollectionAccess.WaitAsync();
            switch (dialog.Result)
            {
                case DialogResult.Add:
                {
                    Manga addedManga = new Manga
                    {
                        Title = title,
                        Id = id,
                        Url = url,
                        Image = new Image {Url = new Url { Original = imageUrl}},
                        LatestRelease = dialog.LatestRelease,
                        LastUpdated = DateTime.Now
                    };
                    if (dialog.AnyScanlator || string.IsNullOrEmpty(dialog.SelectedScanlator))
                        addedManga.AnyScanlator = true;
                    else
                    {
                        addedManga.AnyScanlator = false;
                        addedManga.PreferredScanlator = dialog.SelectedScanlator;
                    }
                    Mangas.Add(addedManga);
                    break;
                }
                case DialogResult.Edit:
                {
                    Manga editedManga = Mangas.First(item => item.Id == id);
                    if (dialog.AnyScanlator || string.IsNullOrEmpty(dialog.SelectedScanlator))
                    {
                        editedManga.AnyScanlator = true;
                        editedManga.PreferredScanlator = string.Empty;
                    }
                    else
                    {
                        editedManga.AnyScanlator = false;
                        editedManga.PreferredScanlator = dialog.SelectedScanlator;
                    }
                    editedManga.Title = title;
                    editedManga.Image = new Image { Url = new Url { Original = imageUrl } };
                    if (!string.IsNullOrEmpty(dialog.LatestRelease.Title) && !string.IsNullOrEmpty(dialog.LatestRelease.Scanlator))
                        editedManga.LatestRelease = dialog.LatestRelease;
                    editedManga.LastUpdated = DateTime.Now;
                    SaveLibrary();
                    break;
                }
                case DialogResult.Remove:
                {
                    Mangas.Remove(Mangas.First(item => item.Id == id));
                    break;
                }
            }
            CollectionAccess.Release();
        }

        private async void CheckManga(object? sender, EventArgs e)
        {
            await CollectionAccess.WaitAsync();
            DateTime time = DateTime.Now;
            foreach (var manga in Mangas)
            {
                bool hasIncompleteInfo = string.IsNullOrEmpty(manga.Title) || string.IsNullOrEmpty(manga.Url)|| string.IsNullOrEmpty(manga.Image.Url.Original);
                bool hasOldInfo = time - manga.LastUpdated >= TimeSpan.FromDays(1);
                bool shouldUpdate = (hasIncompleteInfo || hasOldInfo) && !manga.Completed;
                if (shouldUpdate)
                    _ = GetMangaInfo(manga);
            }
            CollectionAccess.Release();
        }

        private async Task GetMangaInfo(Manga manga)
        {
            if (string.IsNullOrEmpty(manga.Id))
            {
                Logger.Error("Failed to get the manga info because the id is empty.");
                return;
            }

            MangaInfo mangaInfo = await MangaUpdates.GetSeries(manga.Id);
            if(!string.IsNullOrEmpty(mangaInfo.Title))
                manga.Title = mangaInfo.Title;
            if(!string.IsNullOrEmpty(mangaInfo.Url))
                manga.Url = mangaInfo.Url;
            if(!string.IsNullOrEmpty(mangaInfo.Image.Url.Original))
                manga.Image.Url.Original = mangaInfo.Image.Url.Original;
            manga.Completed = mangaInfo.Completed;
            manga.LastUpdated = DateTime.Now;
            SaveLibrary();
            MangasView.Refresh();
        }

        public async void SaveLibrary()
        {
            await CollectionAccess.WaitAsync();
            Library.SaveLibrary(FilePath, Mangas);
            CollectionAccess.Release();
        }
    }
}
