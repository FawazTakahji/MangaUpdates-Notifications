using System;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using ModernWpf.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MangaUpdates_Notifications.Extensions;
using MangaUpdates_Notifications.Native.Enums;
using MangaUpdates_Notifications.Native.Structs;
using MangaUpdates_Notifications.Utilities;
using ModernWpf;
using Path = System.IO.Path;

namespace MangaUpdates_Notifications.Views
{
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();
        }

        private void ToggleWindow_Click(object sender, RoutedEventArgs e)
        {
            ToggleWindow();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        [RelayCommand] private void ToggleWindow()
        {
            if (this.IsVisible)
                this.Hide();
            else
            {
                this.Show();
                this.WindowState = WindowState.Normal;
                this.Activate();
            }
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu contextMenu = (ContextMenu)sender;
            MenuItem menuItem = contextMenu.Items.OfType<MenuItem>().FirstOrDefault();
            if (this.IsVisible)
            {
                menuItem.Header = "Minimize To Tray";
                menuItem.Icon = new PathIcon { Data = (Geometry)Application.Current.FindResource("MinimizeIcon") };
            }
            else
            {
                menuItem.Header = "Show Window";
                menuItem.Icon = new PathIcon { Data = (Geometry)Application.Current.FindResource("MaximizeIcon") };
            }
            contextMenu.UpdateDefaultStyle();
        }

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationView navigationView = (NavigationView)sender;
            Grid paneRoot = (Grid)navigationView.FindDescendantByName("PaneRoot");

            ColumnDefinition col1 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
            ColumnDefinition col2 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) };
            paneRoot.ColumnDefinitions.Add(col1);
            paneRoot.ColumnDefinitions.Add(col2);

            Rectangle redRectangle = new Rectangle
            {
                Fill = (SolidColorBrush)FindResource("SystemControlBackgroundAccentBrush"),
                Width = 5
            };

            Grid.SetColumn(redRectangle, 1);
            paneRoot.Children.Add(redRectangle);
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            if (!Global.Settings.MainView.HasValue)
                return;

            IntPtr handle = new WindowInteropHelper(this).Handle;

            WINDOWPLACEMENT wp = Global.Settings.MainView.Value;
            wp.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
            wp.flags = 0;
            if (wp.showCmd == (int)SW.SHOWMINIMIZED)
                wp.showCmd = (int)SW.SHOWNORMAL;

            Native.Methods.SetWindowPlacement(handle, ref wp);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WINDOWPLACEMENT? wp = this.GetPlacement();

            if (!wp.HasValue)
                return;

            Global.Settings.MainView = wp;
            string settingsPath = Path.Combine(Global.BaseDir, "Settings.json");
            Settings.SaveSettings(settingsPath, Global.Settings);
        }
    }
}