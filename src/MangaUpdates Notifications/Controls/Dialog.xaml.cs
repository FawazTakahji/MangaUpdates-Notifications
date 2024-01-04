using System.Windows;
using System.Windows.Controls;
using ModernWpf.Controls;

namespace MangaUpdates_Notifications.Controls
{
    public partial class Dialog : ContentDialog
    {
        public string MainMessage
        {
            get => (string)GetValue(MainMessageProperty);
            set => SetValue(MainMessageProperty, value);
        }

        public string? ExceptionString
        {
            get => (string?)GetValue(ExceptionStringProperty);
            set => SetValue(ExceptionStringProperty, value);
        }

        public Dialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        public static readonly DependencyProperty MainMessageProperty =
            DependencyProperty.Register(nameof(MainMessage), typeof(string), typeof(Dialog));

        public static readonly DependencyProperty ExceptionStringProperty =
            DependencyProperty.Register(nameof(ExceptionString), typeof(string), typeof(Dialog));

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            TextBlock textBlock = (TextBlock)contextMenu.PlacementTarget;
            Clipboard.SetText(textBlock.Text);
        }
    }
}
