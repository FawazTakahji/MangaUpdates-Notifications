using MangaUpdates_Notifications.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MangaUpdates_Notifications.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void ViewUnloaded(object sender, RoutedEventArgs e)
        {
            SettingsViewModel viewmodel = (SettingsViewModel)DataContext;
            if (viewmodel.RestoreSettingsCommand.CanExecute(null))
                viewmodel.RestoreSettingsCommand.Execute(null);
        }
    }
}
