using MangaUpdates_Notifications.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MangaUpdates_Notifications.Views
{
    public partial class NotificationsView : UserControl
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly TaskCompletionSource<bool> _isViewLoaded = new();
        public NotificationsView()
        {
            InitializeComponent();
        }

        private void View_Loaded(object sender, RoutedEventArgs e)
        {
            _isViewLoaded.TrySetResult(true);
        }

        public async void ScrollToNotification(string title, string chapter, string scanlator)
        {
            await _isViewLoaded.Task;
            Notification item = itemsControl.Items.Cast<Notification>().FirstOrDefault(item => item.Title == title && item.Chapter == chapter && item.Scanlator == scanlator);
            FrameworkElement container = (FrameworkElement)itemsControl.ItemContainerGenerator.ContainerFromItem(item);
            if (container != null)
                container.BringIntoView();
            else
                Logger.Error("Failed to get the item.");
        }
    }
}
