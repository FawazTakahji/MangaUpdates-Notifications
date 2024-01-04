using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MangaUpdates_Notifications.Models;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MangaUpdates_Notifications.ViewModels
{
    public partial class NotificationsViewModel : ObservableObject
    {
        private readonly string _filePath = Path.Combine(Global.BaseDir, "Notifications.json");
        private readonly SemaphoreSlim _collectionAccess = new(1, 1);
        [ObservableProperty] private ObservableCollection<Notification> _notifications = new();

        public NotificationsViewModel()
        {
            Notifications = Utilities.Notifications.LoadNotifications(_filePath);
            Notifications.CollectionChanged += (_, _) => SaveNotifications();
        }

        private async void SaveNotifications()
        {
            await _collectionAccess.WaitAsync();
            Utilities.Notifications.SaveNotifications(_filePath, Notifications);
            _collectionAccess.Release();
        }

        public async Task AddNotification(string title, string imageUrl, string chapter, string scanlator)
        {
            Notification notification = new Notification { Title = title, ImageUrl = imageUrl, Chapter = chapter, Scanlator = scanlator };
            await _collectionAccess.WaitAsync();
            Notifications.Add(notification);
            _collectionAccess.Release();
        }

        [RelayCommand] private async Task RemoveNotification(Notification notification)
        {
            await _collectionAccess.WaitAsync();
            Notifications.Remove(notification);
            _collectionAccess.Release();
        }

        public async Task RemoveNotification(string title, string chapter, string scanlator)
        {
            await _collectionAccess.WaitAsync();
            Notification notification = Notifications.FirstOrDefault(item => item.Title == title && item.Chapter == chapter && item.Scanlator == scanlator);
            Notifications.Remove(notification);
            _collectionAccess.Release();
        }

        [RelayCommand] private async Task ClearNotifications()
        {
            ToastNotificationManagerCompat.Uninstall();
            await _collectionAccess.WaitAsync();
            Notifications.Clear();
            _collectionAccess.Release();
        }
    }
}
