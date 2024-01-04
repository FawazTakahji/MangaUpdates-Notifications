using ModernWpf.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MangaUpdates_Notifications.Enums;

namespace MangaUpdates_Notifications.Controls
{
    public partial class NavigationViewItemEx : NavigationViewItem
    {
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(NavigationViewItemEx));

        public ViewType Type { get; init; }

        public string Id { get; init; }

        public string Text { get; set; }

        public UserControl View { get; set; }

        public UserControl Owner { get; set; }

        public NavigationViewItemEx()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
