using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MangaUpdates_Notifications.Models;
using MangaUpdates_Notifications.Utilities;
using System.Threading.Tasks;

namespace MangaUpdates_Notifications.ViewModels
{
    partial class SearchViewModel : ObservableObject
    {
        [ObservableProperty] private string _searchQuery = string.Empty;
        [ObservableProperty] private Filters _filters = new();
        private string _oldSearchQuery = string.Empty;
        private Filters _oldFilters = new();
        [ObservableProperty] private int _boxNumber = 1;
        [ObservableProperty] private Search _searchData = new();

        [RelayCommand] private async Task SearchText()
        {
            if (string.IsNullOrEmpty(SearchQuery))
                return;

            _oldSearchQuery = SearchQuery;
            _oldFilters = Filters.ShallowCopy();
            SearchData = await MangaUpdates.Search(1, SearchQuery, Filters);
            BoxNumber = SearchData.CurrentPage;
        }

        [RelayCommand] private async Task SearchPage(int page)
        {
            if (string.IsNullOrEmpty(_oldSearchQuery))
                return;

            SearchData = await MangaUpdates.Search(page, _oldSearchQuery, _oldFilters);
            BoxNumber = SearchData.CurrentPage;
        }
    }
}
