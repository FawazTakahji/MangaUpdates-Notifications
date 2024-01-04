using ModernWpf;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MangaUpdates_Notifications.Controls
{
    [INotifyPropertyChanged]
    public partial class ThemeButton : UserControl
    {
        public ApplicationTheme CurrentTheme
        {
            get => (ApplicationTheme)GetValue(CurrentThemeProperty);
            set => SetValue(CurrentThemeProperty, value);
        }

        public static readonly DependencyProperty CurrentThemeProperty =
            DependencyProperty.Register(nameof(CurrentTheme), typeof(ApplicationTheme), typeof(ThemeButton), new PropertyMetadata(ApplicationTheme.Light, OnCurrentThemeChangedCallBack));

        public ThemeButton()
        {
            InitializeComponent();

            if ((ApplicationTheme)ThemeManager.Current.ApplicationTheme == ApplicationTheme.Light)
            {
                tbText.Text = "Light Theme";
                pIcon.Data = (System.Windows.Media.Geometry)FindResource("LightThemeIcon");
            }
            else
            {
                tbText.Text = "Dark Theme";
                pIcon.Data = (System.Windows.Media.Geometry)FindResource("DarkThemeIcon");
            }
        }

        private static void OnCurrentThemeChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
                ((ThemeButton)sender).OnPropertyChanged(nameof(CurrentTheme));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((ApplicationTheme)ThemeManager.Current.ApplicationTheme == ApplicationTheme.Light)
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                tbText.Text = "Dark Theme";
                pIcon.Data = (System.Windows.Media.Geometry)FindResource("DarkThemeIcon");
                CurrentTheme = ApplicationTheme.Dark;
            }
            else
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                tbText.Text = "Light Theme";
                pIcon.Data = (System.Windows.Media.Geometry)FindResource("LightThemeIcon");
                CurrentTheme = ApplicationTheme.Light;
            }
        }
    }
}
