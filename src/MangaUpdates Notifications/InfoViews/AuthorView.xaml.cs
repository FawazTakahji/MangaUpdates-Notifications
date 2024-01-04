using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MangaUpdates_Notifications.Models;
using MangaUpdates_Notifications.Utilities;

namespace MangaUpdates_Notifications.InfoViews
{
    [INotifyPropertyChanged]
    public partial class AuthorView : UserControl
    {
        [ObservableProperty] private string _id;
        [ObservableProperty] private Author _data;
        [ObservableProperty] private Visibility _socialVisibility = Visibility.Collapsed;
        [ObservableProperty] private Visibility _siteVisibility = Visibility.Collapsed;
        [ObservableProperty] private Visibility _twitterVisibility = Visibility.Collapsed;
        [ObservableProperty] private Visibility _facebookVisibility = Visibility.Collapsed;

        public AuthorView()
        {
            InitializeComponent();
            DataContext = this;
        }

        partial void OnIdChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
                _ = GetInfo();
        }

        private async Task GetInfo()
        {
            Data = await MangaUpdates.GetAuthor(Id);

            string pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
            Regex regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (regex.IsMatch(Data.Social.OfficialSite))
                SiteVisibility = Visibility.Visible;
            if (regex.IsMatch(Data.Social.Twitter))
                TwitterVisibility = Visibility.Visible;
            if (regex.IsMatch(Data.Social.Facebook))
                FacebookVisibility = Visibility.Visible;

            SocialVisibility = (SiteVisibility == Visibility.Visible || TwitterVisibility == Visibility.Visible || FacebookVisibility == Visibility.Visible)
                ? Visibility.Visible
                : Visibility.Collapsed;

            if(!string.IsNullOrEmpty(Data.Image.Url.Original))
                Image.Source = await Misc.UrlToBitmapImg(Data.Image.Url.Original);
            if (Image.Source == null)
                Image.Visibility = Visibility.Collapsed;
            Ring.IsActive = false;
            Viewer.Visibility = Visibility.Visible;
        }

        [RelayCommand] private void OpenLink(string url)
        {
            if (!string.IsNullOrEmpty(url))
                Process.Start(new ProcessStartInfo(url) {UseShellExecute = true});
        }
    }
}