// TMDB API İstemcisi

using CinemAI.Models;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;

namespace CinemAI.Services;

public interface ITmdbApiClient
{
    Task<List<Movie>> GetPopularMoviesAsync(int page = 1, string language = "tr-TR");
    Task<List<Movie>> GetMoviesByGenreAsync(int genreId, string language = "tr-TR");
    Task<List<Movie>> SearchMoviesAsync(string query, string language = "tr-TR");
    Task<List<Movie>> GetSimilarMoviesAsync(int movieId, string language = "tr-TR");
    Task<MovieDetails?> GetMovieDetailsAsync(int movieId, string language = "tr-TR");
    Task<MovieCredits?> GetMovieCreditsAsync(int movieId, string language = "tr-TR");
    Task<List<Keyword>> GetMovieKeywordsAsync(int movieId);
    Task<List<Movie>> GetMoviesByCastAsync(int personId, string language = "tr-TR");
    Task<List<Movie>> GetMoviesByCrewAsync(int personId, string language = "tr-TR");
    Task<List<Genre>> GetGenresAsync(string language = "tr-TR");
    Task<Stream?> GetImageStreamAsync(string path);
    Task<List<Movie>> GetMoviesByKeywordsAsync(List<int> keywordIds);
    Task<MovieDetails?> GetMovieFullDetailsAsync(int movieId, string language = "tr-TR");
}

public class TmdbApiClient(HttpClient httpClient, IConfiguration configuration, IMemoryCache cache) : ITmdbApiClient
{
    private readonly string _apiKey = configuration["TmdbApiKey"] ?? "0b80b583f196032d370c854ab3100555";
    private const string BaseUrl = "https://api.themoviedb.org/3";
    private const string FallbackIp = "65.9.175.66";
    private const string FallbackImageIp = "185.93.2.243";

    // ayni anda max 12 istek
    private static readonly SemaphoreSlim _apiSemaphore = new(12);

    // cache sureleri
    private static readonly TimeSpan MovieDetailsCacheDuration = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan GenreCacheDuration = TimeSpan.FromMinutes(60);
    private static readonly TimeSpan DiscoveryCacheDuration = TimeSpan.FromMinutes(10);

    private static List<Genre>? _cachedGenres;

    // film listeleme

    public async Task<List<Movie>> GetPopularMoviesAsync(int page = 1, string language = "tr-TR")
    {
        var cacheKey = $"popular_movies_{language}_{page}";

        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = DiscoveryCacheDuration;
            var response = await GetAsync<MovieResponse>($"/movie/popular?api_key={_apiKey}&language={language}&page={page}");
            return response?.Results ?? [];
        }) ?? [];
    }

    public async Task<List<Movie>> GetMoviesByGenreAsync(int genreId, string language = "tr-TR")
    {
        var response = await GetAsync<MovieResponse>(
            $"/discover/movie?api_key={_apiKey}&language={language}&with_genres={genreId}&sort_by=vote_count.desc");
        return response?.Results ?? [];
    }

    public async Task<List<Movie>> SearchMoviesAsync(string query, string language = "tr-TR")
    {
        var response = await GetAsync<MovieResponse>(
            $"/search/movie?api_key={_apiKey}&language={language}&query={Uri.EscapeDataString(query)}");
        return response?.Results ?? [];
    }

    public async Task<List<Movie>> GetSimilarMoviesAsync(int movieId, string language = "tr-TR")
    {
        var response = await GetAsync<MovieResponse>($"/movie/{movieId}/similar?api_key={_apiKey}&language={language}");
        return response?.Results ?? [];
    }

    // film detaylari

    public async Task<MovieDetails?> GetMovieDetailsAsync(int movieId, string language = "tr-TR") =>
        await GetAsync<MovieDetails>($"/movie/{movieId}?api_key={_apiKey}&language={language}");

    public async Task<MovieCredits?> GetMovieCreditsAsync(int movieId, string language = "tr-TR") =>
        await GetAsync<MovieCredits>($"/movie/{movieId}/credits?api_key={_apiKey}&language={language}");

    // tek api cagrisinda detay + credits + videos alir
    public async Task<MovieDetails?> GetMovieFullDetailsAsync(int movieId, string language = "tr-TR")
    {
        var cacheKey = $"movie_details_{movieId}_{language}";

        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = MovieDetailsCacheDuration;

            var details = await GetAsync<MovieDetailsWithAppends>(
                $"/movie/{movieId}?api_key={_apiKey}&language={language}&append_to_response=credits,videos");

            if (details is null) return null;

            details.Credits = details.AppendedCredits;
            details.Videos = details.AppendedVideos?.Results ?? [];

            return details;
        });
    }

    public async Task<List<Keyword>> GetMovieKeywordsAsync(int movieId)
    {
        var response = await GetAsync<KeywordsResponse>($"/movie/{movieId}/keywords?api_key={_apiKey}");
        return response?.Keywords ?? [];
    }

    // kisi bazli sorgular

    public async Task<List<Movie>> GetMoviesByCastAsync(int personId, string language = "tr-TR")
    {
        var response = await GetAsync<MovieResponse>(
            $"/discover/movie?api_key={_apiKey}&language={language}&with_cast={personId}&sort_by=vote_count.desc");
        return response?.Results ?? [];
    }

    public async Task<List<Movie>> GetMoviesByCrewAsync(int personId, string language = "tr-TR")
    {
        var response = await GetAsync<PersonCreditsResponse>(
            $"/person/{personId}/movie_credits?api_key={_apiKey}&language={language}");

        if (response?.Crew is null) return [];

        return [.. response.Crew
            .Where(c => c.Job == "Director")
            .OrderByDescending(c => c.VoteCount)
            .Take(20)
            .Select(c => new Movie
            {
                Id = c.Id,
                Title = c.Title,
                Overview = c.Overview,
                PosterPath = c.PosterPath,
                ReleaseDate = c.ReleaseDate,
                VoteAverage = c.VoteAverage,
                VoteCount = c.VoteCount,
                GenreIds = c.GenreIds ?? []
            })];
    }

    // yardimci metotlar

    public async Task<List<Genre>> GetGenresAsync(string language = "tr-TR")
    {
        if (_cachedGenres is not null) return _cachedGenres;

        var response = await GetAsync<GenreResponse>($"/genre/movie/list?api_key={_apiKey}&language={language}");
        _cachedGenres = response?.Genres ?? [];
        return _cachedGenres;
    }

    public async Task<Stream?> GetImageStreamAsync(string path)
    {
        try
        {
            var url = $"https://image.tmdb.org/t/p/w500{path}";
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStreamAsync();
        }
        catch { }

        try
        {
            var fallbackUrl = $"https://{FallbackImageIp}/t/p/w500{path}";
            using var request = new HttpRequestMessage(HttpMethod.Get, fallbackUrl);
            request.Headers.Host = "image.tmdb.org";
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStreamAsync();
        }
        catch { }

        return null;
    }

    public async Task<List<Movie>> GetMoviesByKeywordsAsync(List<int> keywordIds)
    {
        if (keywordIds is not { Count: > 0 }) return [];

        var keywordParam = string.Join(",", keywordIds);
        var response = await GetAsync<MovieResponse>(
            $"/discover/movie?api_key={_apiKey}&language=tr-TR&with_keywords={keywordParam}&sort_by=vote_count.desc&vote_count.gte=100");
        return response?.Results ?? [];
    }

    // http istek yardimcisi

    private async Task<T?> GetAsync<T>(string relativePath)
    {
        await _apiSemaphore.WaitAsync();
        try
        {
            try
            {
                var response = await httpClient.GetFromJsonAsync<T>($"{BaseUrl}{relativePath}");
                if (response is not null) return response;
            }
            catch { }

            try
            {
                var uri = new Uri($"{BaseUrl}{relativePath}");
                var fallbackUrl = $"https://{FallbackIp}{uri.PathAndQuery}";
                using var request = new HttpRequestMessage(HttpMethod.Get, fallbackUrl);
                request.Headers.Host = uri.Host;
                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<T>();
            }
            catch { }

            return default;
        }
        finally
        {
            _apiSemaphore.Release();
        }
    }
}

// dto siniflari

public class MovieResponse
{
    public List<Movie> Results { get; init; } = [];
}

public class GenreResponse
{
    public List<Genre> Genres { get; init; } = [];
}

public class KeywordsResponse
{
    [JsonPropertyName("keywords")]
    public List<Keyword> Keywords { get; init; } = [];
}

public class Keyword
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}

public class PersonCreditsResponse
{
    [JsonPropertyName("crew")]
    public List<PersonCrewCredit> Crew { get; init; } = [];
}

public class PersonCrewCredit
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("overview")]
    public string? Overview { get; init; }

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; init; }

    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; init; }

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; init; }

    [JsonPropertyName("vote_count")]
    public int VoteCount { get; init; }

    [JsonPropertyName("genre_ids")]
    public List<int>? GenreIds { get; init; }

    [JsonPropertyName("job")]
    public string Job { get; init; } = string.Empty;
}

public class MovieDetailsWithAppends : MovieDetails
{
    [JsonPropertyName("credits")]
    public MovieCredits? AppendedCredits { get; init; }

    [JsonPropertyName("videos")]
    public VideoResponse? AppendedVideos { get; init; }
}
