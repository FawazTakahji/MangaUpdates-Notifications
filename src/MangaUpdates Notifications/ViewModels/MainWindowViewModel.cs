using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MangaUpdates_Notifications.Controls;
using ModernWpf.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MangaUpdates_Notifications.Enums;
using MangaUpdates_Notifications.InfoViews;
using MangaUpdates_Notifications.Views;

namespace MangaUpdates_Notifications.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty] private UserControl _frameContent;
        [ObservableProperty] private ObservableCollection<Object> _menuItems;
        [ObservableProperty] private ObservableCollection<NavigationViewItem> _footerMenuItems;
        [ObservableProperty] private object _selectedItem;

        private readonly SearchView _searchView = new() { DataContext = new SearchViewModel() };
        private readonly LibraryView _libraryView = new() { DataContext = Global.Library };

        public MainWindowViewModel()
        {
            MenuItems = new ObservableCollection<Object>
            {
                new NavigationViewItem { Content = "Library", Tag = "Library", Icon = new PathIcon { Data = (Geometry)Application.Current.FindResource("LibraryIcon") } },
                new NavigationViewItem { Content = "Search", Tag = "Search", Icon = new PathIcon { Data = (Geometry)Application.Current.FindResource("SearchIcon") } },
                new NavigationViewItemSeparator()
            };
            FooterMenuItems = new ObservableCollection<NavigationViewItem>
            {
                new() { Content = "Notifications", Tag = "Notifications", Icon = new PathIcon { Data = (Geometry)Application.Current.FindResource("NotificationsIcon") } },
                new() { Content = "Settings", Tag = "Settings", Icon = new PathIcon { Data = (Geometry)Application.Current.FindResource("SettingsIcon") } },
                new() { Content = "About", Tag = "About", Icon = new PathIcon { Data = (Geometry)Application.Current.FindResource("InfoIcon") } }
            };
            SelectedItem = MenuItems[0];
        }

        partial void OnSelectedItemChanged(object value)
        {
            if (value is NavigationViewItemEx navViewItemEx)
                FrameContent = navViewItemEx.View;
            else
            {
                var navViewItem = (NavigationViewItem)value;
                FrameContent = (string)navViewItem.Tag switch
                {
                    "Library" => _libraryView,
                    "Search" => _searchView,
                    "Notifications" => new NotificationsView { DataContext = Global.Notifications },
                    "Settings" => new SettingsView { DataContext = new SettingsViewModel() },
                    "About" => new AboutView { DataContext = new AboutViewModel() },
                    _ => _libraryView
                };
            }
        }

        public void Navigate(string view)
        {
            switch (view)
            {
                case "Library":
                case "Search":
                    SelectedItem = MenuItems.OfType<NavigationViewItem>().First(item => (string)item.Tag == view);
                    break;
                case "Notifications":
                case "Settings":
                case "About":
                    SelectedItem = FooterMenuItems.First(item => (string)item.Tag == view);
                    break;
            }
        }

        public void Navigate(ViewType type, string title, string id, UserControl sender = null)
        {
            bool isCtrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            NavigationViewItemEx item = MenuItems.OfType<NavigationViewItemEx>().FirstOrDefault(navItem => navItem.Type == type && navItem.Id == id);

            if (item == null)
            {
                item = CreateItem(type, title, id, sender);
                if (sender == null)
                    MenuItems.Add(item);
                else
                {
                    NavigationViewItemEx senderOwner = MenuItems.OfType<NavigationViewItemEx>().FirstOrDefault(navItem => navItem.View == sender);
                    int index = MenuItems.IndexOf(senderOwner) + 1;
                    while (index < MenuItems.Count && ((NavigationViewItemEx)MenuItems[index]).Owner == sender)
                    {
                        index++;
                    }
                    MenuItems.Insert(index, item);
                }
            }

            if (!isCtrlPressed)
            {
                SelectedItem = item;
            }
        }

        [RelayCommand] private void RemoveItem(object item)
        {
            if (item is NavigationViewItemEx navItem)
            {
                if (SelectedItem == navItem)
                {
                    int index = MenuItems.IndexOf(SelectedItem);
                    if (MenuItems.Count - 1 > index)
                        SelectedItem = MenuItems[index + 1];
                    else
                        SelectedItem = index == 3 ? MenuItems[1] : MenuItems[index - 1];
                }
                MenuItems.Remove(navItem);
            }
        }

        private NavigationViewItemEx CreateItem(ViewType type, string title, string id, UserControl owner = null)
        {
            NavigationViewItemEx item = new NavigationViewItemEx
            {
                Type = type,
                Id = id,
                Text = title,
                Command = RemoveItemCommand,
                Owner = owner
            };
            switch (type)
            {
                case ViewType.Manga:
                    item.View = new MangaView { Id = id };
                    item.Icon = new PathIcon { Data = (Geometry)Application.Current.FindResource("BookIcon") };
                    break;
                case ViewType.Author:
                    item.View = new AuthorView { Id = id };
                    item.Icon = new PathIcon { Data = (Geometry)Application.Current.FindResource("PersonIcon") };
                    break;
                case ViewType.Publisher:
                    item.View = new PublisherView { Id = id };
                    item.Icon = new PathIcon { Data = (Geometry)Application.Current.FindResource("CompanyIcon") };
                    break;
            }
            return item;
        }
    }
}
