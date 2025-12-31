// TMDB Servis Arayuzu

using CinemAI.Models;

namespace CinemAI.Services;

public interface ITmdbService
{
    Task<List<Movie>> GetPopularMoviesAsync(string language = "tr-TR");
    Task<List<Movie>> GetMoviesByGenreAsync(int genreId, string language = "tr-TR");
    Task<List<Movie>> SearchMoviesAsync(string query, string language = "tr-TR");
    Task<List<Genre>> GetGenresAsync();

    Task<MovieDetails?> GetMovieDetailsAsync(int movieId, string language = "tr-TR");
    Task<MovieCredits?> GetMovieCreditsAsync(int movieId);

    Task<List<Movie>> GetMoviesByCastAsync(int personId, string language = "tr-TR");
    Task<List<Movie>> GetMoviesByCrewAsync(int personId, string language = "tr-TR");

    Task<Stream?> GetImageStreamAsync(string path);

    Task<List<RecommendationResult>> GetEnhancedRecommendationsAsync(
        Dictionary<int, int> movieRatings,
        List<Movie> ratedMovies,
        string language = "tr-TR");
}
