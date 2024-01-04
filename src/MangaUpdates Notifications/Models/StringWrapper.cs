using CommunityToolkit.Mvvm.ComponentModel;

namespace MangaUpdates_Notifications.Models
{
    public partial class StringWrapper : ObservableObject
    {
        [ObservableProperty] private string _string = string.Empty;
        public string Genre { set { String = value; } }
        public string Category { set { String = value; } }
        public string as_string { set { String = value; } }
    }
}
