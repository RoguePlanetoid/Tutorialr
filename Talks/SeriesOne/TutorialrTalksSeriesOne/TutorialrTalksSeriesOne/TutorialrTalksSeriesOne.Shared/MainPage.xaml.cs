using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace TutorialrTalksSeriesOne
{
    public sealed partial class MainPage : Page
    {
        private readonly Client _client = new Client("GB");

        private async void Navigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            Spinner.IsActive = true;
            var results = new List<Result>();
            Background.Source = null;
            switch (args.SelectedItemContainer.Tag.ToString())
            {
                case "category":
                    results = await _client.ListCategoriesAsync();
                    break;
                case "featured":
                    results = await _client.ListPlaylistsAsync();
                    break;
                case "released":
                    results = await _client.ListAlbumsAsync();
                    break;
            }
            Values.ItemsSource = results;
            Spinner.IsActive = false;
        }

        private async void NavigateResult(Result result)
        {
            Spinner.IsActive = true;
            switch (result.Type)
            {
                case ResultType.Category:
                    Values.ItemsSource = await _client.ListPlaylistsAsync(result.Id);
                    Background.Source = new BitmapImage(new Uri(result.Image));
                    break;
                case ResultType.Playlist:
                    Values.ItemsSource = await _client.ListPlaylistItemsAsync(result.Id);
                    Background.Source = new BitmapImage(new Uri(result.Image));
                    break;
                case ResultType.Album:
                    var results = await _client.ListTracksAsync(result.Id);
                    results.ForEach(f => f.Image = result.Image);
                    Values.ItemsSource = results;
                    Background.Source = new BitmapImage(new Uri(result.Image));
                    break;
            }
            Spinner.IsActive = false;
        }

        public MainPage()
        {
            this.InitializeComponent();
            _client.Action = (item) => NavigateResult(item);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) =>
            Navigation.SelectedItem = Navigation.MenuItems.First();
    }
}
