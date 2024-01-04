using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Xaml.Behaviors;

namespace MangaUpdates_Notifications.Behaviors
{
    public class VerticalScrollBarEdit : Behavior<ScrollViewer>
    {
        public Thickness? Margin
        {
            get => (Thickness?)GetValue(MarginProperty);
            set => SetValue(MarginProperty, value);
        }

        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register(nameof(Margin), typeof(Thickness?), typeof(VerticalScrollBarEdit),
                new PropertyMetadata(null));

        protected override void OnAttached()
        {
            AssociatedObject.Loaded += Edit;
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            ScrollBar verticalScrollBar = (ScrollBar)scrollViewer.Template.FindName("PART_VerticalScrollBar", scrollViewer);

            if (Margin != null)
                verticalScrollBar.Margin = (Thickness)Margin;

            AssociatedObject.Loaded -= Edit;
            Detach();
        }
    }
}