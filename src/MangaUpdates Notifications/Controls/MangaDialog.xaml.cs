using CommunityToolkit.Mvvm.ComponentModel;
using MangaUpdates_Notifications.Models;
using MangaUpdates_Notifications.Utilities;
using ModernWpf.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using MangaUpdates_Notifications.Enums;

namespace MangaUpdates_Notifications.Controls
{
    [INotifyPropertyChanged]
    public partial class MangaDialog : ContentDialog
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [ObservableProperty] private bool _isLoading = true;
        [ObservableProperty] private Visibility _cbVisibility = Visibility.Collapsed;
        [ObservableProperty] private bool _inLibrary;
        [ObservableProperty] private bool _anyScanlator = true;
        [ObservableProperty] private string _mangaId;
        [ObservableProperty] private ObservableCollection<string> _scanlators;
        [ObservableProperty] private string _selectedScanlator;
        [ObservableProperty] private Manga.Release _latestRelease;
        public DialogResult Result;

        public MangaDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        partial void OnMangaIdChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
                GetInfo();
        }

        private async void GetInfo()
        {
            LatestRelease = await MangaUpdates.GetSeriesLatestRelease(MangaId);

            Scanlators = new ObservableCollection<string>(await MangaUpdates.GetSeriesScanlators(MangaId));
            if (Scanlators.Count > 0)
            {
                SelectedScanlator = Scanlators.Where(item => item == SelectedScanlator).DefaultIfEmpty(Scanlators.First()).FirstOrDefault() ?? string.Empty;
                CbVisibility = Visibility.Visible;
            }

            IsLoading = false;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            HideDialog(InLibrary ? DialogResult.Edit : DialogResult.Add);
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            HideDialog(DialogResult.Remove);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void HideDialog(DialogResult result)
        {
            Result = result;
            Hide();
        }
    }
}
