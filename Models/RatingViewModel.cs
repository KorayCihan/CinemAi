// Puanlama ViewModel

namespace CinemAI.Models;

public class RatingViewModel
{
    public List<Movie> Movies { get; set; } = [];
    public List<RecommendationResult> RecommendedMovies { get; set; } = [];

    public string? FavoriteGenreName { get; set; }
    public List<string> FavoriteGenres { get; set; } = [];
    public List<string> FavoriteActors { get; set; } = [];
    public List<string> FavoriteDirectors { get; set; } = [];

    public bool HasRecommendations => RecommendedMovies.Count > 0;

    public void ParseReasons()
    {
        if (RecommendedMovies.Count == 0) return;

        var allReasons = RecommendedMovies
            .SelectMany(r => r.Reasons)
            .Distinct()
            .ToList();

        FavoriteGenres = ExtractNames(allReasons, "🎭", ["Tür", "Genre"]);
        FavoriteActors = ExtractNames(allReasons, "⭐", ["Oyuncu", "Actor"]);
        FavoriteDirectors = ExtractNames(allReasons, "🎬", ["Yönetmen", "Director"]);

        FavoriteGenreName = FavoriteGenres.FirstOrDefault();
    }

    private static List<string> ExtractNames(List<string> reasons, string emoji, string[] prefixes) =>
        reasons
            .Where(r => r.StartsWith(emoji))
            .Select(r =>
            {
                var cleaned = r;
                foreach (var prefix in prefixes)
                    cleaned = cleaned.Replace($"{emoji} {prefix}: ", "");
                return cleaned.Replace($"{emoji} ", "");
            })
            .Distinct()
            .ToList();
}
