using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Spotify.NetStandard.Client;
using Spotify.NetStandard.Client.Interfaces;
using Spotify.NetStandard.Requests;
using Spotify.NetStandard.Enums;

namespace SpotifyForDevelopers.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private const string client = "clientid";
        private const string secret = "clientsecret";
        private const string state = "spotify.workshop";
        private const string token = "token";
        private const string country = "GB";

        public static readonly ISpotifyApi Api = SpotifyClientFactory.CreateSpotifyClient(client, secret).Api;

        public Token Token { get; set; }
        public string Value { get; set; }
        public string Option { get; set; }
        public bool Flag { get; set; }
        public IFormFile Upload { get; set; }
        public IEnumerable<Result> Results { get; set; }
        public Uri RedirectUri => new Uri($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}");
        public Uri CurrentUri => new Uri($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}");

        public void LoadToken()
        {
            if (Request.Cookies[token] != null)
                Token = new Token(Request.Cookies[token]);
        }

        public void SaveToken()
        {
            if (Token != null)
                Response.Cookies.Append(token, Token.ToString());
        }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(
            string code = null, string access_token = null)
        {
            LoadToken();
            if (code != null)
                Token = new Token(await Api.GetAuthorisationCodeAuthTokenAsync(
                    CurrentUri, RedirectUri, state));
            if (access_token != null)
                Token = new Token(Api.GetImplicitGrantAuthToken(
                    CurrentUri, RedirectUri, state));
            SaveToken();
            return Page();
        }

        public IActionResult OnPostAuthorisationCode() =>
            Redirect(Api.GetAuthorisationCodeAuthUri(
                RedirectUri, state, Scope.AllPermissions).ToString());

        public IActionResult OnPostImplicitGrant() =>
            Redirect(Api.GetImplicitGrantAuthUri(
                RedirectUri, state, Scope.AllPermissions).ToString());

        public async Task<IActionResult> OnPostClientCredentialsAsync()
        {
            Token = new Token(await Api.GetClientCredentialsAuthTokenAsync());
            SaveToken();
            return Page();
        }

        public async Task<IActionResult> OnPostSearchForAnItemAsync(string value, string option)
        {
            LoadToken();
            var results = new List<Result>();
            var search = await Api.SearchForItemAsync(value,
            new SearchType()
            {
                Album = option.Contains("Album"),
                Track = option.Contains("Track"),
                Artist = option.Contains("Artist"),
                Playlist = option.Contains("Playlist"),
                Show = option.Contains("Show"),
                Episode = option.Contains("Episode")
            }, country);
            if (search?.Albums?.Items != null)
            {
                results.AddRange(search.Albums.Items.Select(result => new Result(
                    result.Id, result.Name, result?.Images?.FirstOrDefault()?.Url,
                    new Result(result?.Artists[0]?.Id, result?.Artists[0]?.Name))));
            }
            if (search?.Tracks?.Items != null)
            {
                results.AddRange(search.Tracks.Items.Select(result => new Result(
                    result.Id, result.Name, result.Album?.Images?.FirstOrDefault()?.Url,
                    new Result(result?.Artists[0]?.Id, result?.Artists[0]?.Name))));
            }
            if (search?.Artists?.Items != null)
            {
                results.AddRange(search.Artists.Items.Select(result => new Result(
                    result.Id, result.Name, result?.Images?.FirstOrDefault()?.Url)));
            }
            if (search?.Playlists?.Items != null)
            {
                results.AddRange(search.Playlists.Items.Select(result => new Result(
                    result.Id, result.Name, result?.Images?.FirstOrDefault()?.Url)));
            }
            if (search?.Shows?.Items != null)
            {
                results.AddRange(search.Shows.Items.Select(result => new Result(
                    result.Id, result.Name, result?.Images?.FirstOrDefault()?.Url)));
            }
            if (search?.Episodes?.Items != null)
            {
                results.AddRange(search.Episodes.Items.Select(result => new Result(
                    result.Id, result.Name, result?.Images?.FirstOrDefault()?.Url)));
            }
            if (results.Any())
                Results = results;
            return Page();
        }

        public async Task<IActionResult> OnPostGetAllCategoriesAsync()
        {
            LoadToken();
            var results = await Api.GetAllCategoriesAsync(country);
            if (results?.Items != null)
            {
                Results = results.Items.Select(result => new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result.Images?.FirstOrDefault()?.Url
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetCategoryAsync(string value)
        {
            LoadToken();
            var result = await Api.GetCategoryAsync(value);
            if (result != null)
            {
                Results = new List<Result> { new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result.Images?.FirstOrDefault()?.Url
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetCategoryPlaylistsAsync(string value)
        {
            LoadToken();
            var results = await Api.GetCategoryPlaylistsAsync(value);
            if (results?.Items != null)
            {
                Results = results.Items.Select(result => new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Images?.FirstOrDefault()?.Url
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetRecommendationGenresAsync()
        {
            LoadToken();
            var results = await Api.GetRecommendationGenresAsync();
            if (results?.Genres != null)
            {
                Results = results.Genres.Select(result => new Result()
                {
                    Id = result
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetRecommendationsAsync(string value)
        {
            LoadToken();
            var results = await Api.GetRecommendationsAsync(
                seedGenres: new List<string> { value });
            if (results?.Tracks != null)
            {
                Results = results.Tracks.Select(result => new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Inner = new Result()
                    {
                        Id = result?.Artists?.FirstOrDefault()?.Id,
                        Name = result?.Artists?.FirstOrDefault()?.Name
                    }
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetAllNewReleasesAsync()
        {
            LoadToken();
            var results = await Api.GetAllNewReleasesAsync(country);
            if (results?.Items != null)
            {
                Results = results.Items.Select(result => new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result.Images?.FirstOrDefault()?.Url,
                    Inner = new Result()
                    {
                        Id = result?.Artists?.FirstOrDefault()?.Id,
                        Name = result?.Artists?.FirstOrDefault()?.Name
                    }
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetAllFeaturedPlaylistsAsync()
        {
            LoadToken();
            var results = await Api.GetAllFeaturedPlaylistsAsync(country);
            if (results?.Items != null)
            {
                Results = results.Items.Select(result => new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Images?.FirstOrDefault()?.Url
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetMultipleArtistsAsync(string value)
        {
            LoadToken();
            var values = value.Split(",").ToList();
            var results = await Api.GetMultipleArtistsAsync(values);
            if (results != null)
            {
                Results = results.Select(result => new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Images?.FirstOrDefault()?.Url
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetArtistAsync(string value)
        {
            LoadToken();
            var result = await Api.GetArtistAsync(value);
            if (result != null)
            {
                Results = new List<Result> { new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Images?.FirstOrDefault()?.Url
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetArtistAlbumsAsync(string value)
        {
            LoadToken();
            var results = await Api.GetArtistAlbumsAsync(value);
            if (results?.Items != null)
            {
                Results = results.Items.Select(result => new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Images?.FirstOrDefault()?.Url
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetArtistTopTracksAsync(string value)
        {
            LoadToken();
            var results = await Api.GetArtistTopTracksAsync(value, country);
            if (results != null)
            {
                Results = results.Select(result => new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Album?.Images?.FirstOrDefault()?.Url,
                    Inner = new Result()
                    {
                        Id = result?.Album?.Id,
                        Name = result?.Album?.Name
                    }
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetArtistRelatedArtistsAsync(string value)
        {
            LoadToken();
            var results = await Api.GetArtistRelatedArtistsAsync(value);
            if (results != null)
            {
                Results = results.Select(result => new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Images?.FirstOrDefault()?.Url
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetMultipleAlbumsAsync(string value)
        {
            LoadToken();
            var values = value.Split(",").ToList();
            var results = await Api.GetMultipleAlbumsAsync(values);
            if (results != null)
            {
                Results = results.Select(result => new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Images?.FirstOrDefault()?.Url
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetAlbumAsync(string value)
        {
            LoadToken();
            var result = await Api.GetAlbumAsync(value);
            if (result != null)
            {
                Results = new List<Result> { new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Images?.FirstOrDefault()?.Url,
                    Inner = new Result()
                    {
                        Id = result?.Artists?.FirstOrDefault()?.Id,
                        Name = result?.Artists?.FirstOrDefault()?.Name
                    }
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetAlbumTracksAsync(string value)
        {
            LoadToken();
            var results = await Api.GetAlbumTracksAsync(value, country);
            if (results?.Items != null)
            {
                Results = results.Items.Select(result => new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Album?.Images?.FirstOrDefault()?.Url,
                    Inner = new Result()
                    {
                        Id = result?.Artists?.FirstOrDefault()?.Id,
                        Name = result?.Artists?.FirstOrDefault()?.Name
                    }
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetSeveralTracksAsync(string value)
        {
            LoadToken();
            var values = value.Split(",").ToList();
            var results = await Api.GetSeveralTracksAsync(values);
            if (results != null)
            {
                Results = results.Select(result => new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Album?.Images?.FirstOrDefault()?.Url
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetTrackAsync(string value)
        {
            LoadToken();
            var result = await Api.GetTrackAsync(value);
            if (result != null)
            {
                Results = new List<Result> { new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Album?.Images?.FirstOrDefault()?.Url,
                    Inner = new Result()
                    {
                        Id = result?.Artists?.FirstOrDefault()?.Id,
                        Name = result?.Artists?.FirstOrDefault()?.Name
                    }
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetAudioFeaturesForSeveralTracksAsync(string value)
        {
            LoadToken();
            var values = value.Split(",").ToList();
            var results = await Api.GetSeveralTracksAudioFeaturesAsync(values);
            if (results != null)
            {
                Results = results.Select(result => new Result()
                {
                    Name = System.Text.Json.JsonSerializer.Serialize(result,
                        new System.Text.Json.JsonSerializerOptions()
                        { WriteIndented = true })
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetAudioFeaturesForTrackAsync(string value)
        {
            LoadToken();
            var result = await Api.GetTrackAudioFeaturesAsync(value);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Name = System.Text.Json.JsonSerializer.Serialize(result,
                        new System.Text.Json.JsonSerializerOptions()
                        { WriteIndented = true })
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetTrackAudioAnalysisAsync(string value)
        {
            LoadToken();
            var result = await Api.GetTrackAudioAnalysisAsync(value);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Name = System.Text.Json.JsonSerializer.Serialize(result,
                        new System.Text.Json.JsonSerializerOptions()
                        { WriteIndented = true })
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetEpisodeAsync(string value)
        {
            LoadToken();
            var result = await Api.GetEpisodeAsync(value, country);
            if (result != null)
            {
                Results = new List<Result> { new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Images?.FirstOrDefault()?.Url
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetMultipleEpisodesAsync(string value)
        {
            LoadToken();
            var values = value.Split(",").ToList();
            var results = await Api.GetMultipleEpisodesAsync(values, country);
            if (results != null)
            {
                Results = results.Select(result => new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Images?.FirstOrDefault()?.Url
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetShowAsync(string value)
        {
            LoadToken();
            var result = await Api.GetShowAsync(value, country);
            if (result != null)
            {
                Results = new List<Result> { new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Images?.FirstOrDefault()?.Url
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetMultipleShowsAsync(string value)
        {
            LoadToken();
            var values = value.Split(",").ToList();
            var results = await Api.GetMultipleShowsAsync(values, country);
            if (results != null)
            {
                Results = results.Select(result => new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Images?.FirstOrDefault()?.Url
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetShowEpisodesAsync(string value)
        {
            LoadToken();
            var results = await Api.GetShowEpisodesAsync(value, country);
            if (results != null)
            {
                Results = results.Items.Select(result => new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Images?.FirstOrDefault()?.Url
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostFollowArtistsOrUsersAsync(string value, FollowType option)
        {
            LoadToken();
            var values = value.Split(",").ToList();
            var result = await Api.FollowArtistsOrUsersAsync(values, option);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostFollowPlaylistAsync(string value)
        {
            LoadToken();
            var result = await Api.FollowPlaylistAsync(value);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetFollowingStateForArtistsOrUsersAsync(string value, FollowType option)
        {
            LoadToken();
            var values = value.Split(",").ToList();
            var results = await Api.GetFollowingStateForArtistsOrUsersAsync(values, option);
            if (results != null)
            {
                Results = results.Select(result => new Result()
                {
                    Name = result.ToString()
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostCheckUsersFollowingPlaylistAsync(string value, string option)
        {
            LoadToken();
            var values = value.Split(",").ToList();
            var results = await Api.CheckUsersFollowingPlaylistAsync(values, option);
            if (results != null)
            {
                Results = results.Select(result => new Result()
                {
                    Name = result.ToString()
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetUsersFollowedArtistsAsync()
        {
            LoadToken();
            var results = await Api.GetUsersFollowedArtistsAsync();
            if (results != null)
            {
                Results = results.Items.Select(result => new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Images?.FirstOrDefault()?.Url
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUnfollowArtistsOrUsersAsync(string value, FollowType option)
        {
            LoadToken();
            var values = value.Split(",").ToList();
            var result = await Api.UnfollowArtistsOrUsersAsync(values, option);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUnfollowPlaylistAsync(string value)
        {
            LoadToken();
            var result = await Api.UnfollowPlaylistAsync(value);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetPlaylistAsync(string value)
        {
            LoadToken();
            var result = await Api.GetPlaylistAsync(value);
            if (result != null)
            {
                Results = new List<Result> { new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Images?.FirstOrDefault()?.Url
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetPlaylistTracksAsync(string value)
        {
            LoadToken();
            var results = await Api.GetPlaylistTracksAsync(value);
            if (results?.Items != null)
            {
                Results = results.Items.Select(result => new Result()
                {
                    Id = result?.Track.Id,
                    Name = result?.Track.Name,
                    Image = result?.Track.Album?.Images?.FirstOrDefault()?.Url,
                    Inner = new Result()
                    {
                        Id = result?.Track?.Artists?.FirstOrDefault()?.Id,
                        Name = result?.Track?.Artists?.FirstOrDefault()?.Name
                    }
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostCreatePlaylistAsync(string value, string option)
        {
            LoadToken();
            var result = await Api.CreatePlaylistAsync(value, option);
            if (result != null)
            {
                Results = new List<Result> { new Result()
                {
                    Id = result.Id,
                    Name = result.Name
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetCurrentUserPlaylistsAsync()
        {
            LoadToken();
            var results = await Api.GetUserPlaylistsAsync();
            if (results?.Items != null)
            {
                Results = results.Items.Select(result => new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Images?.FirstOrDefault()?.Url
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetUserPlaylistsAsync(string value)
        {
            LoadToken();
            var results = await Api.GetUserPlaylistsAsync(value);
            if (results?.Items != null)
            {
                Results = results.Items.Select(result => new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                    Image = result?.Images?.FirstOrDefault()?.Url
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAddTracksToPlaylistAsync(string value, string option)
        {
            LoadToken();
            var values = value.Split(",").Select(value => $"spotify:track:{value}").ToList();
            var result = await Api.AddTracksToPlaylistAsync(option, values);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.SnapshotId,
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveTracksFromPlaylistAsync(string value, string option)
        {
            LoadToken();
            var values = value.Split(",").Select(value => $"spotify:track:{value}").ToList();
            var result = await Api.RemoveTracksFromPlaylistAsync(option, values);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.SnapshotId,
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUploadCustomPlaylistCoverImageAsync(string value)
        {
            LoadToken();
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            Upload.OpenReadStream().CopyTo(stream);
            var file = stream.ToArray();
            var result = await Api.UploadCustomPlaylistCoverImageAsync(value, file);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.Code.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetPlaylistCoverImageAsync(string value)
        {
            LoadToken();
            var results = await Api.GetPlaylistCoverImageAsync(value);
            if (results != null)
            {
                Results = results.Select(result => new Result()
                {
                    Image = result.Url
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostChangePlaylistDetailsAsync(string value, string option)
        {
            LoadToken();
            var result = await Api.ChangePlaylistDetailsAsync(value, option);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.Code.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostReplacePlaylistTracksAsync(string value, string option)
        {
            LoadToken();
            var values = value.Split(",").Select(value => $"spotify:track:{value}").ToList();
            var result = await Api.ReplacePlaylistTracksAsync(option, values);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.Code.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostReorderPlaylistTracksAsync(string value, string option)
        {
            LoadToken();
            var index = int.Parse(option);
            var result = await Api.ReorderPlaylistTracksAsync(value, index, 0, 1);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.SnapshotId.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostSaveUserAlbumsAsync(string value)
        {
            LoadToken();
            var values = value.Split(",").ToList();
            var result = await Api.SaveUserAlbumsAsync(values);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.Code.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostSaveUserTracksAsync(string value)
        {
            LoadToken();
            var values = value.Split(",").ToList();
            var result = await Api.SaveUserTracksAsync(values);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.Code.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostSaveUserShowsAsync(string value)
        {
            LoadToken();
            var values = value.Split(",").ToList();
            var result = await Api.SaveUserShowsAsync(values);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.Code.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostCheckUserSavedAlbumsAsync(string value)
        {
            LoadToken();
            var values = value.Split(",").ToList();
            var results = await Api.CheckUserSavedAlbumsAsync(values);
            if (results != null)
            {
                Results = results.Select(result => new Result()
                {
                    Name = result.ToString()
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostCheckUserSavedTracksAsync(string value)
        {
            LoadToken();
            var values = value.Split(",").ToList();
            var results = await Api.CheckUserSavedTracksAsync(values);
            if (results != null)
            {
                Results = results.Select(result => new Result()
                {
                    Name = result.ToString()
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostCheckUserSavedShowsAsync(string value)
        {
            LoadToken();
            var values = value.Split(",").ToList();
            var results = await Api.CheckUserSavedShowsAsync(values);
            if (results != null)
            {
                Results = results.Select(result => new Result()
                {
                    Name = result.ToString()
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetUserSavedAlbumsAsync()
        {
            LoadToken();
            var results = await Api.GetUserSavedAlbumsAsync();
            if (results?.Items != null)
            {
                Results = results.Items.Select(result => new Result()
                {
                    Id = result.Album.Id,
                    Name = result.Album.Name,
                    Image = result?.Album?.Images?.FirstOrDefault()?.Url,
                    Inner = new Result()
                    {
                        Id = result?.Album?.Artists?.FirstOrDefault()?.Id,
                        Name = result?.Album?.Artists?.FirstOrDefault()?.Name
                    }
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetUserSavedTracksAsync()
        {
            LoadToken();
            var results = await Api.GetUserSavedTracksAsync();
            if (results?.Items != null)
            {
                Results = results.Items.Select(result => new Result()
                {
                    Id = result.Track.Id,
                    Name = result.Track.Name,
                    Image = result?.Track?.Album?.Images?.FirstOrDefault()?.Url,
                    Inner = new Result()
                    {
                        Id = result?.Track?.Album?.Artists?.FirstOrDefault()?.Id,
                        Name = result?.Track?.Album?.Artists?.FirstOrDefault()?.Name
                    }
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetUserSavedShowsAsync()
        {
            LoadToken();
            var results = await Api.GetUserSavedShowsAsync();
            if (results?.Items != null)
            {
                Results = results.Items.Select(result => new Result()
                {
                    Id = result.Show.Id,
                    Name = result.Show.Name,
                    Image = result?.Show?.Images?.FirstOrDefault()?.Url
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveUserAlbumsAsync(string value)
        {
            LoadToken();
            var values = value.Split(",").ToList();
            var result = await Api.RemoveUserAlbumsAsync(values);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.Code.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveUserTracksAsync(string value)
        {
            LoadToken();
            var values = value.Split(",").ToList();
            var result = await Api.RemoveUserTracksAsync(values);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.Code.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveUserShowsAsync(string value)
        {
            LoadToken();
            var values = value.Split(",").ToList();
            var result = await Api.RemoveUserShowsAsync(values);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.Code.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUserPlaybackAddToQueueAsync(string value, string option)
        {
            LoadToken();
            var uri = $"spotify:{option.ToLower()}:{value}";
            var result = await Api.UserPlaybackAddToQueueAsync(uri);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.Code.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUserPlaybackStartResumeAsync(string value, string option)
        {
            LoadToken();
            var isTrack = option.Equals("Track");
            var uri = $"spotify:{option.ToLower()}:{value}";
            var uris = new List<string> { uri };
            var result = await Api.UserPlaybackStartResumeAsync(
                contextUri: isTrack ? null : uri,
                uris: isTrack ? uris : null,
                offsetPosition: 0, position: 0);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.Code.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUserPlaybackPauseAsync()
        {
            LoadToken();
            var result = await Api.UserPlaybackPauseAsync();
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.Code.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUserPlaybackNextTrackAsync()
        {
            LoadToken();
            var result = await Api.UserPlaybackNextTrackAsync();
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.Code.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUserPlaybackPreviousTrackAsync()
        {
            LoadToken();
            var result = await Api.UserPlaybackPreviousTrackAsync();
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.Code.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUserPlaybackSeekTrackAsync(string option)
        {
            LoadToken();
            var position = (int)TimeSpan.FromSeconds(int.Parse(option)).TotalMilliseconds;
            var result = await Api.UserPlaybackSeekTrackAsync(position);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.Code.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUserPlaybackSetVolumeAsync(string option)
        {
            LoadToken();
            var percent = int.Parse(option);
            var result = await Api.UserPlaybackSetVolumeAsync(percent);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.Code.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUserPlaybackToggleShuffleAsync(bool flag)
        {
            LoadToken();
            var result = await Api.UserPlaybackToggleShuffleAsync(flag);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.Code.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUserPlaybackSetRepeatModeAsync(bool flag)
        {
            var state = flag ? RepeatState.Track : RepeatState.Off;
            var result = await Api.UserPlaybackSetRepeatModeAsync(state);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.Code.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetUserPlaybackCurrentAsync()
        {
            LoadToken();
            var result = await Api.GetUserPlaybackCurrentAsync(country,
                new List<string> { "track", "episode" });
            if (result != null)
            {
                if(result.Track != null)
                {
                    Results = new List<Result> { new Result()
                    {
                        Id = result?.Track?.Id,
                        Name = result?.Track?.Name,
                        Image = result?.Track.Album?.Images?.FirstOrDefault()?.Url,
                        Inner = new Result()
                        {
                            Id = result?.Track?.Artists?.FirstOrDefault()?.Id,
                            Name = result?.Track?.Artists?.FirstOrDefault()?.Name
                        }
                    }};
                }
                if (result.Episode != null)
                {
                    Results = new List<Result> { new Result()
                    {
                        Id = result?.Episode?.Id,
                        Name = result?.Episode?.Name,
                        Image = result?.Episode?.Images?.FirstOrDefault()?.Url,
                        Inner = new Result()
                        {
                            Id = result?.Episode?.Show?.Id,
                            Name = result?.Episode?.Show?.Name
                        }
                    }};
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetUserPlaybackCurrentTrackAsync()
        {
            LoadToken();
            var result = await Api.GetUserPlaybackCurrentTrackAsync(country,
                new List<string> { "track", "episode" });
            if (result != null)
            {
                if (result.Track != null)
                {
                    Results = new List<Result> { new Result()
                    {
                        Id = result?.Track?.Id,
                        Name = result?.Track?.Name,
                        Image = result?.Track.Album?.Images?.FirstOrDefault()?.Url,
                        Inner = new Result()
                        {
                            Id = result?.Track?.Artists?.FirstOrDefault()?.Id,
                            Name = result?.Track?.Artists?.FirstOrDefault()?.Name
                        }
                    }};
                }
                if (result.Episode != null)
                {
                    Results = new List<Result> { new Result()
                    {
                        Id = result?.Episode?.Id,
                        Name = result?.Episode?.Name,
                        Image = result?.Episode?.Images?.FirstOrDefault()?.Url,
                        Inner = new Result()
                        {
                            Id = result?.Episode?.Show?.Id,
                            Name = result?.Episode?.Show?.Name
                        }
                    }};
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetUserRecentlyPlayedTracksAsync()
        {
            LoadToken();
            var results = await Api.GetUserRecentlyPlayedTracksAsync();
            if (results?.Items != null)
            {
                Results = results.Items.Select(result => new Result()
                {
                    Id = result.Track.Id,
                    Name = result.Track.Name,
                    Image = result?.Track.Album?.Images?.FirstOrDefault()?.Url,
                    Inner = new Result()
                    {
                        Id = result?.Track?.Artists?.FirstOrDefault()?.Id,
                        Name = result?.Track?.Artists?.FirstOrDefault()?.Name
                    }
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetUserPlaybackDevicesAsync()
        {
            LoadToken();
            var results = await Api.GetUserPlaybackDevicesAsync();
            if (results?.Items != null)
            {
                Results = results.Items.Select(result => new Result()
                {
                    Id = result.Id,
                    Name = result.Name,
                });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUserPlaybackTransferAsync(string value)
        {
            LoadToken();
            var deviceIds = new List<string> { value };
            var result = await Api.UserPlaybackTransferAsync(deviceIds, true);
            if (result != null)
            {
                Results = new List<Result>() { new Result()
                {
                    Id = result.Code.ToString(),
                    Name = result.Success.ToString()
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetUserTopArtistsAndTracksAsync(string option)
        {
            LoadToken();
            if(option.Equals("Artists"))
            {
                var results = await Api.GetUserTopArtistsAsync();
                if (results?.Items != null)
                {
                    Results = results.Items.Select(result => new Result()
                    {
                        Id = result.Id,
                        Name = result.Name,
                        Image = result?.Images?.FirstOrDefault()?.Url
                    });
                }
            }
            else
            {
                var results = await Api.GetUserTopTracksAsync();
                if (results?.Items != null)
                {
                    Results = results.Items.Select(result => new Result()
                    {
                        Id = result.Id,
                        Name = result.Name,
                        Image = result?.Album?.Images?.FirstOrDefault()?.Url,
                        Inner = new Result()
                        {
                            Id = result?.Album?.Artists?.FirstOrDefault()?.Id,
                            Name = result?.Album?.Artists?.FirstOrDefault()?.Name
                        }
                    });
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetUserProfileAsync(string value)
        {
            LoadToken();
            var result = await Api.GetUserProfileAsync(value);
            if (result != null)
            {
                Results = new List<Result> { new Result()
                {
                    Id = result.Id,
                    Name = result.DisplayName,
                    Image = result?.Images?.FirstOrDefault()?.Url
                }};
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetCurrentUserProfileAsync()
        {
            LoadToken();
            var result = await Api.GetUserProfileAsync();
            if (result != null)
            {
                Results = new List<Result> { new Result()
                {
                    Id = result.Id,
                    Name = result.DisplayName,
                    Image = result?.Images?.FirstOrDefault()?.Url
                }};
            }
            return Page();
        }
    }
}