using System.Diagnostics;
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
    public partial class PublisherView : UserControl
    {
        [ObservableProperty] private string _id;
        [ObservableProperty] private Publisher _data;

        public PublisherView()
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
            Data = await MangaUpdates.GetPublisher(Id);
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
