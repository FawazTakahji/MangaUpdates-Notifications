using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using MangaUpdates_Notifications.Enums;
using MangaUpdates_Notifications.Models;
using MangaUpdates_Notifications.Utilities;

namespace MangaUpdates_Notifications.Controls
{
    [INotifyPropertyChanged]
    public partial class MangaCard : UserControl
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        [ObservableProperty] private ImageSource? _mangaImage;
        [ObservableProperty] private bool? _inLibrary;
        [ObservableProperty] private bool _isCompleted;
        [ObservableProperty] private Geometry _buttonIcon;
        [ObservableProperty] private string _toolTipText;

        public string Id
        {
            get => (string)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string ImageUrl
        {
            get => (string)GetValue(ImageUrlProperty);
            set => SetValue(ImageUrlProperty, value);
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register(nameof(Id), typeof(string), typeof(MangaCard),
                new FrameworkPropertyMetadata("", (d, _) => ((MangaCard)d).CheckMangaStatus()));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(MangaCard));

        public static readonly DependencyProperty ImageUrlProperty =
            DependencyProperty.Register(nameof(ImageUrl), typeof(string), typeof(MangaCard),
                new FrameworkPropertyMetadata(string.Empty, (d, _) => ((MangaCard)d).GetImage()));

        public MangaCard()
        {
            InitializeComponent();
            Global.Library.Mangas.CollectionChanged += (_, _) => CheckMangaStatus();
        }

        partial void OnInLibraryChanged(bool? value)
        {
            if (value == true)
            {
                ButtonIcon = (Geometry)FindResource("EditIcon");
                ToolTipText = "Edit";
            }
            else
            {
                ButtonIcon = (Geometry)FindResource("AddIcon");
                ToolTipText = "Add to library";
            }
        }

        private async void GetImage()
        {
            if (string.IsNullOrEmpty(ImageUrl))
            {
                BrokenImage();
                Logger.Warn($"There is no image for the manga \"{Id}\".");
                return;
            }

            MangaImage = await Misc.UrlToBitmapImg(ImageUrl);
            if (MangaImage != null)
            {
                prImage.Visibility = Visibility.Collapsed;
                prImage.IsActive = false;
            }
            else
            {
                BrokenImage();
                Logger.Error($"Failed to get the image for the manga \"{Id}\".");
            }
        }

        private void CheckMangaStatus()
        {
            Manga? manga = Global.Library.Mangas.FirstOrDefault(item => item.Id == Id);
            InLibrary = manga != null;
            IsCompleted = manga?.Completed ?? false;
        }

        private void BrokenImage()
        {
            prImage.Visibility = Visibility.Collapsed;
            prImage.IsActive = false;
            pathBrokenImage.Visibility = Visibility.Visible;
        }

        private void AddEdit_Click(object sender, RoutedEventArgs e)
        {
            Manga manga = DataContext is Manga ? (Manga)DataContext : ((Search.Result)DataContext).Record;
            if (string.IsNullOrEmpty(manga.Id))
            {
                _ = Misc.ShowDialog("Manga id is not valid.");
                return;
            }

            if (!string.IsNullOrEmpty(manga.Id))
                Global.Library.AddEditManga(manga.Title, manga.Id, manga.Url, manga.Image.Url.Original, (bool)InLibrary);
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Id))
                Global.MainViewModel.Navigate(ViewType.Manga, !string.IsNullOrEmpty(Title) ? Title : Id, Id);
            else
                _ = Misc.ShowDialog("Manga id is not valid.");
        }
    }
}
