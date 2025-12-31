// TMDB API Yanit Modeli

using System.Text.Json.Serialization;

namespace CinemAI.Models;

public class TmdbResponse
{
    [JsonPropertyName("page")]
    public int Page { get; init; }

    [JsonPropertyName("results")]
    public List<Movie> Results { get; init; } = [];

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; init; }

    [JsonPropertyName("total_results")]
    public int TotalResults { get; init; }
}
