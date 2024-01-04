using MangaUpdates_Notifications.ViewModels;
using ModernWpf;
using ModernWpf.Controls;
using System.Windows;
using System.Windows.Controls;

namespace MangaUpdates_Notifications.Views
{
    public partial class SearchView : UserControl
    {
        private bool _buttonSet;

        public SearchView()
        {
            InitializeComponent();
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion == null)
            {
                Filters.Visibility = Visibility.Collapsed;
                SearchViewModel viewmodel = (SearchViewModel)DataContext;
                if (viewmodel.SearchTextCommand.CanExecute(null))
                    viewmodel.SearchTextCommand.Execute(null);
            }
        }

        private void NumberBox_Loaded(object sender, RoutedEventArgs e)
        {
            NumberBox numberBox = (NumberBox)sender;
            numberBox.Minimum = 1;
        }

        private void AutoSuggestBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (_buttonSet)
                return;

            AutoSuggestBox autoSuggestBox = (AutoSuggestBox)sender;
            Button button = (Button)autoSuggestBox.FindDescendantByName("FilterButton");
            button.Click += (_, _) => ToggleFilters();
            _buttonSet = true;
        }

        private void ToggleFilters()
        {
            Filters.Visibility = Filters.IsVisible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}