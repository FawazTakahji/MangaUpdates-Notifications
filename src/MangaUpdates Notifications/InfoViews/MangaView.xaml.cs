using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MangaUpdates_Notifications.Enums;
using MangaUpdates_Notifications.Models;
using MangaUpdates_Notifications.Utilities;

namespace MangaUpdates_Notifications.InfoViews
{
    [INotifyPropertyChanged]
    public partial class MangaView : UserControl
    {
        [ObservableProperty] private string _id;
        [ObservableProperty] private MangaInfo _data;
        [ObservableProperty] private Visibility _animeVisibility = Visibility.Collapsed;
        [ObservableProperty] private Visibility _statusVisibility = Visibility.Collapsed;
        [ObservableProperty] private Geometry _buttonIcon;
        [ObservableProperty] private string _buttonText;

        public MangaView()
        {
            InitializeComponent();
            DataContext = this;
            Global.Library.Mangas.CollectionChanged += (_, _) => IsMangaInLibrary();
        }

        private void IsMangaInLibrary()
        {
            if (Global.Library.Mangas.Any(item => item.Id == Id))
            {
                ButtonIcon = (Geometry)FindResource("EditIcon");
                ButtonText = "Edit";
            }
            else
            {
                ButtonIcon = (Geometry)FindResource("AddIcon");
                ButtonText = "Add to Library";
            }
        }

        partial void OnIdChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _ = GetInfo();
                IsMangaInLibrary();
            }
        }

        private async Task GetInfo()
        {
            Data = await MangaUpdates.GetSeries(Id);

            if(!string.IsNullOrEmpty(Data.Image.Url.Original))
                Image.Source = await Misc.UrlToBitmapImg(Data.Image.Url.Original);
            if (Image.Source == null)
                Image.Visibility = Visibility.Collapsed;
            if (!string.IsNullOrEmpty(Data.Anime.Start) || !string.IsNullOrEmpty(Data.Anime.End))
                AnimeVisibility = Visibility.Visible;
            if (!string.IsNullOrEmpty(Data.Status) || !string.IsNullOrEmpty(Data.LicensedText) || !string.IsNullOrEmpty(Data.CompletedText))
                StatusVisibility = Visibility.Visible;

            Ring.IsActive = false;
            Viewer.Visibility = Visibility.Visible;
        }

        private void AddEdit_Click(object sender, RoutedEventArgs e)
        {
            bool inLibrary = Global.Library.Mangas.Any(item => item.Id == Id);
            Global.Library.AddEditManga(Data.Title, Data.Id, Data.Url, Data.Image.Url.Original, inLibrary);
        }

        private void Browser_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Data.Url))
                Process.Start(new ProcessStartInfo(Data.Url) { UseShellExecute = true });
        }

        [RelayCommand] private void OpenAuthor(Author author)
        {
            if (!string.IsNullOrEmpty(author.Id))
                Global.MainViewModel.Navigate(ViewType.Author, !string.IsNullOrEmpty(author.Name) ? author.Name : author.Id, author.Id, this);
        }

        [RelayCommand] private void OpenManga(object manga)
        {
            if (manga is Recommendation recommendation)
            {
                if (!string.IsNullOrEmpty(recommendation.Id))
                    Global.MainViewModel.Navigate(ViewType.Manga,
                        !string.IsNullOrEmpty(recommendation.Title) ? recommendation.Title : recommendation.Id, recommendation.Id, this);
            }
            else if (manga is RelatedSeries relatedSeries)
            {
                if (!string.IsNullOrEmpty(relatedSeries.Id))
                    Global.MainViewModel.Navigate(ViewType.Manga,
                        !string.IsNullOrEmpty(relatedSeries.Title) ? relatedSeries.Title
                            : relatedSeries.Id, relatedSeries.Id, this);
            }
        }

        [RelayCommand] private void OpenPublisher(Publisher publisher)
        {
            if (!string.IsNullOrEmpty(publisher.Id))
                Global.MainViewModel.Navigate(ViewType.Publisher, !string.IsNullOrEmpty(publisher.Name) ? publisher.Name : publisher.Id, publisher.Id, this);
        }
    }
}
