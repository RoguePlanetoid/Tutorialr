using Spotify.NetStandard.Client;
using Spotify.NetStandard.Client.Interfaces;
using Spotify.NetStandard.Enums;
using Spotify.NetStandard.Requests;
using Spotify.NetStandard.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if __WASM__
using System.Net.Http;
using Uno.UI.Wasm;
#endif

namespace TutorialrTalksSeriesOne
{
    public class Client
    {
        private readonly string _country;
        private readonly Page _page = new Page() { Limit = 50 };
        private readonly NavigateType _navigate = NavigateType.Next;

#if __WASM__
        private readonly ISpotifyClient _client =
           SpotifyClientFactory.CreateSpotifyClient(new HttpClient(new WasmHttpHandler()), Config.ClientId, Config.ClientSecret);
#else
        private readonly ISpotifyClient _client =
           SpotifyClientFactory.CreateSpotifyClient(Config.ClientId, Config.ClientSecret);
#endif

        #region Private Methods
        private string GetImageUrl(List<Image> images) =>
            images?.OrderByDescending(o => o.Height * o.Width)?.FirstOrDefault()?.Url;

        private Paging<TObject> AsPaging<TObject>(List<TObject> list) =>
            list != null ? new Paging<TObject>() { Items = list, Total = list.Count } : null;

        private Result AsResult<TObject>(TObject response)
        {
            var result = new Result();
            switch (response)
            {
                case Category category:
                    result.Id = category?.Id;
                    result.Title = category?.Name;
                    result.Image = GetImageUrl(category?.Images);
                    result.Type = ResultType.Category;
                    break;
                case Album album:
                    result.Id = album?.Id;
                    result.Title = album?.Name;
                    result.Image = GetImageUrl(album?.Images);
                    result.Type = ResultType.Album;
                    break;
                case Artist artist:
                    result.Id = artist?.Id;
                    result.Title = artist?.Name;
                    result.Image = GetImageUrl(artist?.Images);
                    break;
                case Track track:
                    result.Id = track?.Id;
                    result.Title = track?.Name;
                    result.Image = GetImageUrl(track?.Album?.Images);
                    result.Type = ResultType.Item;
                    break;
                case SimplifiedPlaylist playlist:
                    result.Id = playlist?.Id;
                    result.Title = playlist?.Name;
                    result.Image = GetImageUrl(playlist?.Images);
                    result.Type = ResultType.Playlist;
                    break;
                case PlaylistTrack playlistItem:
                    if (playlistItem.Track != null)
                    {
                        result.Id = playlistItem.Track?.Id;
                        result.Title = playlistItem.Track?.Name;
                        result.Image = GetImageUrl(playlistItem.Track?.Album?.Images);
                    }
                    if (playlistItem.Episode != null)
                    {
                        result.Id = playlistItem.Episode?.Id;
                        result.Title = playlistItem.Episode?.Name;
                        result.Image = GetImageUrl(playlistItem.Episode?.Images);
                    }
                    result.Type = ResultType.Item;
                    break;
                default:
                    throw new NotSupportedException();
            }
            result.Command = Action != null ? new Command<Result>(Action) : null;
            return result;
        }

        #endregion Private Methods

        #region Paging Methods
        private async Task<Paging<Category>> PagingCategoriesAsync(string country = null) =>
            (await _client.LookupAllCategoriesAsync(country, page: _page))?.Categories;

        private async Task<Paging<Category>> PagingCategoriesAsync(Paging<Category> paging) =>
            (await _client.NavigateAsync(paging: paging, _navigate))?.Categories;

        private async Task<Paging<Artist>> PagingArtistsAsync(string id) =>
            AsPaging((await _client.LookupArtistRelatedArtistsAsync(id))?.Artists);

        private async Task<Paging<Artist>> PagingArtistsAsync(Paging<Artist> paging) =>
            await _client.PagingAsync(paging: paging, _navigate);

        private async Task<Paging<Album>> PagingAlbumsAsync(string country = null) =>
            (await _client.LookupNewReleasesAsync(country: country, page: _page))?.Albums;

        private async Task<Paging<Album>> PagingAlbumsAsync(Paging<Album> paging) =>
            (await _client.NavigateAsync(paging: paging, _navigate))?.Albums;

        private async Task<Paging<Track>> PagingTracksAsync(string id, string country = null) =>
            await _client.LookupAsync<Paging<Track>>(id, LookupType.AlbumTracks, country);

        private async Task<Paging<Track>> PagingTracksAsync(Paging<Track> paging) =>
            await _client.PagingAsync(paging: paging, _navigate);

        private async Task<Paging<SimplifiedPlaylist>> PagingPlaylistsAsync(string id = null, string country = null) =>
            id != null ? 
                (await _client.LookupAsync<ContentResponse>(id, LookupType.CategoriesPlaylists, country, page: _page))?.Playlists :
                (await _client.LookupFeaturedPlaylistsAsync(country, page: _page))?.Playlists;

        private async Task<Paging<SimplifiedPlaylist>> PagingPlaylistsAsync(Paging<SimplifiedPlaylist> paging) =>
            (await _client.NavigateAsync(paging: paging, _navigate))?.Playlists;

        private async Task<Paging<PlaylistTrack>> PagingPlaylistItemsAsync(string id, string country = null) =>
           await _client.LookupPlaylistItemsAsync(id, country, _page);

        private async Task<Paging<PlaylistTrack>> PagingPlaylistItemsAsync(Paging<PlaylistTrack> paging) =>
            await _client.PagingAsync(paging: paging, _navigate);

        private async Task<Paging<TObject>> PagingAsync<TObject>(string id = null, string country = null)
            where TObject : class
        {
            switch (typeof(TObject))
            {
                case Type category when category == typeof(Category):
                    return await PagingCategoriesAsync(country) as Paging<TObject>;
                case Type artist when artist == typeof(Artist):
                    return await PagingArtistsAsync(id) as Paging<TObject>;
                case Type album when album == typeof(Album):
                    return await PagingAlbumsAsync(country) as Paging<TObject>;
                case Type track when track == typeof(Track):
                    return await PagingTracksAsync(id, country) as Paging<TObject>;
                case Type playlist when playlist == typeof(SimplifiedPlaylist):
                    return await PagingPlaylistsAsync(id, country) as Paging<TObject>;
                case Type item when item == typeof(PlaylistTrack):
                    return await PagingPlaylistItemsAsync(id, country) as Paging<TObject>;
                default:
                    throw new NotSupportedException();
            }
        }

        private async Task<Paging<TObject>> PagingAsync<TObject>(Paging<TObject> paging)
            where TObject : class
        {
            switch (typeof(TObject))
            {
                case Type category when category == typeof(Category):
                    return await PagingCategoriesAsync(paging as Paging<Category>) as Paging<TObject>;
                case Type artist when artist == typeof(Artist):
                    return await PagingArtistsAsync(paging as Paging<Artist>) as Paging<TObject>;
                case Type album when album == typeof(Album):
                    return await PagingAlbumsAsync(paging as Paging<Album>) as Paging<TObject>;
                case Type track when track == typeof(Track):
                    return await PagingTracksAsync(paging as Paging<Track>) as Paging<TObject>;
                case Type playlist when playlist == typeof(SimplifiedPlaylist):
                    return await PagingPlaylistsAsync(paging as Paging<SimplifiedPlaylist>) as Paging<TObject>;
                case Type item when item == typeof(PlaylistTrack):
                    return await PagingPlaylistItemsAsync(paging as Paging<PlaylistTrack>) as Paging<TObject>;
                default:
                    throw new NotSupportedException();
            }
        }
        #endregion Paging Methods

        #region List Methods
        private async Task<List<TObject>> ListAsync<TObject>(string id = null, string country = null)
            where TObject : class
        {
            Paging<TObject> paging = null;
            var list = new List<TObject>();
            do
            {
                paging = paging == null ? await PagingAsync<TObject>(id, country) : await PagingAsync(paging);
                if (paging?.Items != null)
                    paging.Items.ForEach(f => list.Add(f));
            } while (paging?.Items != null);
            return list;
        }

        private async Task<List<Result>> ListResultAsync<TObject>(string id = null, string country = null)
            where TObject : class =>
            (await ListAsync<TObject>(id, country)).Select(s => AsResult(s)).ToList();
        #endregion List Methods

        #region Public Methods
        public Client(string country) => 
            _country = country;

        public async Task<List<Result>> ListCategoriesAsync() =>
            await ListResultAsync<Category>(country: _country);

        public async Task<List<Result>> ListArtistsAsync(string id) =>
            await ListResultAsync<Artist>(id, _country);

        public async Task<List<Result>> ListAlbumsAsync(string id = null) =>
            await ListResultAsync<Album>(id, _country);

        public async Task<List<Result>> ListTracksAsync(string id) =>
            await ListResultAsync<Track>(id, _country);

        public async Task<List<Result>> ListPlaylistsAsync(string id = null) =>
            await ListResultAsync<SimplifiedPlaylist>(id, _country);

        public async Task<List<Result>> ListPlaylistItemsAsync(string id) =>
            await ListResultAsync<PlaylistTrack>(id, _country);
        #endregion Public Methods

        #region Public Properties
        public Action<Result> Action { get; set; }
        #endregion Public Properties
    }
}
