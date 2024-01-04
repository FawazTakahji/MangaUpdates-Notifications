using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MangaUpdates_Notifications.Extensions;
using MangaUpdates_Notifications.Utilities;
using System.Threading.Tasks;

namespace MangaUpdates_Notifications.ViewModels
{
    public partial class AboutViewModel : ObservableObject
    {
        [ObservableProperty] string versionText = string.Empty;

        public AboutViewModel()
        {
            versionText = "v" + typeof(AboutViewModel).Assembly.GetName().Version.GetMajorMinorBuild().ToString();
        }

        [RelayCommand] private static async Task CheckForUpdate()
        {
            await Updates.Check();
        }
    }
}
