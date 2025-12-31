// TMDB Facade Servisi

using CinemAI.Models;

namespace CinemAI.Services;

public class TmdbService(ITmdbApiClient apiClient, IRecommendationEngine recommendationEngine) : ITmdbService
{
    // film listeleme

    public async Task<List<Movie>> GetPopularMoviesAsync(string language = "tr-TR")
    {
        var page1Task = apiClient.GetPopularMoviesAsync(1, language);
        var page2Task = apiClient.GetPopularMoviesAsync(2, language);

        await Task.WhenAll(page1Task, page2Task);

        return [.. page1Task.Result, .. page2Task.Result];
    }

    public Task<List<Movie>> GetMoviesByGenreAsync(int genreId, string language = "tr-TR") =>
        apiClient.GetMoviesByGenreAsync(genreId, language);

    public Task<List<Movie>> SearchMoviesAsync(string query, string language = "tr-TR") =>
        apiClient.SearchMoviesAsync(query, language);

    // film detaylari

    public Task<MovieDetails?> GetMovieDetailsAsync(int movieId, string language = "tr-TR") =>
        apiClient.GetMovieFullDetailsAsync(movieId, language);

    public Task<MovieCredits?> GetMovieCreditsAsync(int movieId) =>
        apiClient.GetMovieCreditsAsync(movieId);

    // kisi bazli sorgular

    public Task<List<Movie>> GetMoviesByCastAsync(int personId, string language = "tr-TR") =>
        apiClient.GetMoviesByCastAsync(personId, language);

    public Task<List<Movie>> GetMoviesByCrewAsync(int personId, string language = "tr-TR") =>
        apiClient.GetMoviesByCrewAsync(personId, language);

    // yardimci metotlar

    public Task<List<Genre>> GetGenresAsync() => apiClient.GetGenresAsync();

    public Task<Stream?> GetImageStreamAsync(string path) => apiClient.GetImageStreamAsync(path);

    // oneri sistemi

    public Task<List<RecommendationResult>> GetEnhancedRecommendationsAsync(
        Dictionary<int, int> movieRatings,
        List<Movie> ratedMovies,
        string language = "tr-TR") =>
        recommendationEngine.GetRecommendationsAsync(movieRatings, ratedMovies, language);
}
