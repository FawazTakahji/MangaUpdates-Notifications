using CommunityToolkit.Mvvm.ComponentModel;

namespace MangaUpdates_Notifications.Models
{
    public partial class Notification : ObservableObject
    {
        [ObservableProperty] private string _title = string.Empty;
        [ObservableProperty] private string _imageUrl = string.Empty;
        [ObservableProperty] private string _chapter = string.Empty;
        [ObservableProperty] private string _scanlator = string.Empty;
    }
}
