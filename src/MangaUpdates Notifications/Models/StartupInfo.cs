using CommunityToolkit.Mvvm.ComponentModel;

namespace MangaUpdates_Notifications.Models
{
    public partial class StartupInfo : ObservableObject
    {
        [ObservableProperty] private bool _inStartup;
        [ObservableProperty] private bool _portable;
        [ObservableProperty] private bool _minimized;
    }
}