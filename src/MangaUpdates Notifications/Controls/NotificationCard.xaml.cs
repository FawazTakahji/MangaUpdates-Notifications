using CommunityToolkit.Mvvm.ComponentModel;
using MangaUpdates_Notifications.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MangaUpdates_Notifications.Controls
{
    [INotifyPropertyChanged]
    public partial class NotificationCard : UserControl
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        [ObservableProperty] private ImageSource? _mangaImage;

        public string MangaTitle
        {
            get => (string)GetValue(MangaTitleProperty);
            set => SetValue(MangaTitleProperty, value);
        }

        public string ImageUrl
        {
            get => (string)GetValue(ImageUrlProperty);
            set => SetValue(ImageUrlProperty, value);
        }

        public string Chapter
        {
            get => (string)GetValue(ChapterProperty);
            set => SetValue(ChapterProperty, value);
        }

        public string Scanlator
        {
            get => (string)GetValue(ScanlatorProperty);
            set => SetValue(ScanlatorProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => (object)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty MangaTitleProperty =
            DependencyProperty.Register(nameof(MangaTitle), typeof(string), typeof(NotificationCard));

        public static readonly DependencyProperty ImageUrlProperty =
            DependencyProperty.Register(nameof(ImageUrl), typeof(string), typeof(NotificationCard), new FrameworkPropertyMetadata(string.Empty, OnImageUrlChanged));

        public static readonly DependencyProperty ChapterProperty =
            DependencyProperty.Register(nameof(Chapter), typeof(string), typeof(NotificationCard));

        public static readonly DependencyProperty ScanlatorProperty =
            DependencyProperty.Register(nameof(Scanlator), typeof(string), typeof(NotificationCard));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(NotificationCard));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(NotificationCard));

        public NotificationCard()
        {
            InitializeComponent();
        }

        private static async void OnImageUrlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NotificationCard instance = (NotificationCard)d;
            if (string.IsNullOrEmpty((string)e.NewValue))
            {
                instance.BrokenImage();
                Logger.Warn($"There is no image for \"{instance.MangaTitle}\".");
                return;
            }

            instance.MangaImage = await Misc.UrlToBitmapImg((string)e.NewValue);
            if (instance.MangaImage != null)
            {
                instance.prImage.Visibility = Visibility.Collapsed;
                instance.prImage.IsActive = false;
            }
            else
            {
                instance.BrokenImage();
                Logger.Error($"Failed to get the image for \"{instance.MangaTitle}\".");
            }
        }

        private void BrokenImage()
        {
            prImage.Visibility = Visibility.Collapsed;
            prImage.IsActive = false;
            pathBrokenImage.Visibility = Visibility.Visible;
        }
    }
}
