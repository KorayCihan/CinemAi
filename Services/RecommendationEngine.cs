// Oneri Motoru

using CinemAI.Models;
using System.Collections.Concurrent;

namespace CinemAI.Services;

public interface IRecommendationEngine
{
    Task<List<RecommendationResult>> GetRecommendationsAsync(
        Dictionary<int, int> movieRatings,
        List<Movie> ratedMovies,
        string language = "tr-TR");
}

public class RecommendationEngine(ITmdbApiClient apiClient) : IRecommendationEngine
{
    private const double BonusSameDirector = 4.0;
    private const double BonusSameActor = 2.5;
    private const double BonusSameGenre = 2.0;
    private const int MinVotes = 100;
    private const double MeanVote = 6.9;

    public async Task<List<RecommendationResult>> GetRecommendationsAsync(
        Dictionary<int, int> movieRatings,
        List<Movie> ratedMovies,
        string language = "tr-TR")
    {
        var recommendations = new ConcurrentDictionary<int, RecommendationResult>();
        var ratedMovieIds = movieRatings.Keys.ToHashSet();

        var highRatedIds = movieRatings
            .Where(r => r.Value >= 4)
            .OrderByDescending(r => r.Value)
            .Take(5)
            .Select(r => r.Key)
            .DefaultIfEmpty(movieRatings.Keys.FirstOrDefault())
            .Where(id => id != 0)
            .ToList();

        if (highRatedIds.Count == 0) return [];

        // paralel api cagrilari
        var detailsTasks = highRatedIds.Select(id => apiClient.GetMovieDetailsAsync(id, language));
        var creditsTasks = highRatedIds.Select(id => apiClient.GetMovieCreditsAsync(id, language));

        var allDetails = await Task.WhenAll(detailsTasks);
        var allCredits = await Task.WhenAll(creditsTasks);

        var processingTasks = highRatedIds.Select(async (movieId, index) =>
        {
            var userRating = movieRatings[movieId];
            var ratingWeight = userRating / 5.0;
            var details = allDetails[index];
            var credits = allCredits[index];

            var tasks = new List<Task>();

            if (credits?.Crew?.FirstOrDefault(c => c.Job == "Director") is { } director)
            {
                tasks.Add(ProcessDirectorMoviesAsync(
                    director, ratingWeight, language, ratedMovieIds, recommendations));
            }

            if (credits?.Cast is { Count: > 0 })
            {
                foreach (var actor in credits.Cast.Take(3))
                {
                    tasks.Add(ProcessActorMoviesAsync(
                        actor, ratingWeight, language, ratedMovieIds, recommendations));
                }
            }

            if (details?.Genres is { Count: > 0 })
            {
                foreach (var genre in details.Genres.Take(2))
                {
                    tasks.Add(ProcessGenreMoviesAsync(
                        genre, ratingWeight, language, ratedMovieIds, recommendations));
                }
            }

            await Task.WhenAll(tasks);
        });

        await Task.WhenAll(processingTasks);

        return [.. recommendations.Values.OrderByDescending(r => r.Score).Take(15)];
    }

    // yardimci metotlar

    private async Task ProcessDirectorMoviesAsync(
        Crew director,
        double ratingWeight,
        string language,
        HashSet<int> ratedMovieIds,
        ConcurrentDictionary<int, RecommendationResult> recommendations)
    {
        var movies = await apiClient.GetMoviesByCrewAsync(director.Id, language);
        var reason = $"🎬 {(language == "en-US" ? "Director" : "Yönetmen")}: {director.Name}";

        foreach (var movie in movies.Where(m => !ratedMovieIds.Contains(m.Id)))
        {
            AddRecommendation(recommendations, movie, BonusSameDirector * ratingWeight, reason);
        }
    }

    private async Task ProcessActorMoviesAsync(
        Cast actor,
        double ratingWeight,
        string language,
        HashSet<int> ratedMovieIds,
        ConcurrentDictionary<int, RecommendationResult> recommendations)
    {
        var movies = await apiClient.GetMoviesByCastAsync(actor.Id, language);
        var roleWeight = actor.Order < 3 ? 1.0 : 0.6;
        var reason = $"⭐ {(language == "en-US" ? "Actor" : "Oyuncu")}: {actor.Name}";

        foreach (var movie in movies.Where(m => !ratedMovieIds.Contains(m.Id)))
        {
            AddRecommendation(recommendations, movie, BonusSameActor * ratingWeight * roleWeight, reason);
        }
    }

    private async Task ProcessGenreMoviesAsync(
        Genre genre,
        double ratingWeight,
        string language,
        HashSet<int> ratedMovieIds,
        ConcurrentDictionary<int, RecommendationResult> recommendations)
    {
        var movies = await apiClient.GetMoviesByGenreAsync(genre.Id, language);
        var reason = $"🎭 {(language == "en-US" ? "Genre" : "Tür")}: {genre.Name}";

        foreach (var movie in movies.Where(m => !ratedMovieIds.Contains(m.Id)))
        {
            AddRecommendation(recommendations, movie, BonusSameGenre * ratingWeight, reason);
        }
    }

    private static void AddRecommendation(
        ConcurrentDictionary<int, RecommendationResult> recommendations,
        Movie movie,
        double bonus,
        string reason)
    {
        if (movie.VoteCount < 50) return;

        var wr = (movie.VoteCount / (double)(movie.VoteCount + MinVotes)) * movie.VoteAverage +
                 (MinVotes / (double)(movie.VoteCount + MinVotes)) * MeanVote;

        var finalScore = (wr / 2) + bonus;

        recommendations.AddOrUpdate(
            movie.Id,
            new RecommendationResult
            {
                Movie = movie,
                Score = finalScore,
                Reasons = [reason]
            },
            (_, existing) =>
            {
                existing.Score = Math.Max(existing.Score, finalScore);
                if (!existing.Reasons.Contains(reason))
                    existing.Reasons.Add(reason);
                return existing;
            });
    }
}