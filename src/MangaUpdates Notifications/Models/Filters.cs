using CommunityToolkit.Mvvm.ComponentModel;

namespace MangaUpdates_Notifications.Models
{
    public partial class Filters : ObservableObject
    {
        [ObservableProperty] private bool _artbook = true;
        [ObservableProperty] private bool _doujinshi = true;
        [ObservableProperty] private bool _manga = true;
        [ObservableProperty] private bool _manhua = true;
        [ObservableProperty] private bool _manhwa = true;
        [ObservableProperty] private bool _novel = true;

        public Filters ShallowCopy()
        {
            return (Filters)this.MemberwiseClone();
        }
    }
}