// Film Modeli

using System.Text.Json.Serialization;

namespace CinemAI.Models;

public class Movie
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("genre_ids")]
    public List<int> GenreIds { get; init; } = [];

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; init; }

    [JsonPropertyName("overview")]
    public string? Overview { get; init; }

    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; init; }

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; init; }

    [JsonPropertyName("vote_count")]
    public int VoteCount { get; init; }

    [JsonIgnore]
    public int UserScore { get; set; }

    public string GetPosterUrl(string size = "w500") =>
        string.IsNullOrEmpty(PosterPath)
            ? "https://via.placeholder.com/500x750?text=No+Image"
            : $"/images{PosterPath}";

    public string GetYear() =>
        string.IsNullOrEmpty(ReleaseDate) || ReleaseDate.Length < 4
            ? string.Empty
            : ReleaseDate[..4];
}
