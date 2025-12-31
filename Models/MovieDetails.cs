// Film Detay Modeli

using System.Text.Json.Serialization;

namespace CinemAI.Models;

public class MovieDetails
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("original_title")]
    public string OriginalTitle { get; init; } = string.Empty;

    [JsonPropertyName("tagline")]
    public string? Tagline { get; init; }

    [JsonPropertyName("overview")]
    public string? Overview { get; init; }

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; init; }

    [JsonPropertyName("backdrop_path")]
    public string? BackdropPath { get; init; }

    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; init; }

    [JsonPropertyName("runtime")]
    public int? Runtime { get; init; }

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; init; }

    [JsonPropertyName("vote_count")]
    public int VoteCount { get; init; }

    [JsonPropertyName("budget")]
    public long Budget { get; init; }

    [JsonPropertyName("revenue")]
    public long Revenue { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("genres")]
    public List<Genre> Genres { get; init; } = [];

    [JsonPropertyName("production_companies")]
    public List<ProductionCompany> ProductionCompanies { get; init; } = [];

    [JsonPropertyName("spoken_languages")]
    public List<SpokenLanguage> SpokenLanguages { get; init; } = [];

    [JsonIgnore]
    public MovieCredits? Credits { get; set; }

    [JsonIgnore]
    public List<Video> Videos { get; set; } = [];

    // yardimci metotlar

    public string GetPosterUrl(string size = "w500") =>
        string.IsNullOrEmpty(PosterPath)
            ? "https://via.placeholder.com/500x750?text=No+Image"
            : $"/images{PosterPath}";

    public string GetBackdropUrl() =>
        string.IsNullOrEmpty(BackdropPath) ? "" : $"/images{BackdropPath}";

    public string GetYear() =>
        string.IsNullOrEmpty(ReleaseDate) || ReleaseDate.Length < 4
            ? "Bilinmiyor"
            : ReleaseDate[..4];

    public string GetFormattedRuntime()
    {
        if (!Runtime.HasValue || Runtime.Value == 0) return "Bilinmiyor";

        var hours = Runtime.Value / 60;
        var minutes = Runtime.Value % 60;
        return hours > 0 ? $"{hours}s {minutes}dk" : $"{minutes}dk";
    }

    public string GetGenreNames() =>
        Genres.Count == 0 ? "Bilinmiyor" : string.Join(", ", Genres.Select(g => g.Name));

    public string? GetTrailerUrl() =>
        Videos.FirstOrDefault(v => v.Site == "YouTube" && v.Type is "Trailer" or "Teaser") is { } trailer
            ? $"https://www.youtube.com/watch?v={trailer.Key}"
            : null;

    public string? GetTrailerEmbedUrl() =>
        Videos.FirstOrDefault(v => v.Site == "YouTube" && v.Type is "Trailer" or "Teaser") is { } trailer
            ? $"https://www.youtube.com/embed/{trailer.Key}"
            : null;

    public string GetDirector() =>
        Credits?.Crew.FirstOrDefault(c => c.Job == "Director")?.Name ?? "Bilinmiyor";

    public List<Cast> GetTopCast(int count = 6) =>
        Credits?.Cast.Take(count).ToList() ?? [];
}

public class ProductionCompany
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("logo_path")]
    public string? LogoPath { get; init; }

    [JsonPropertyName("origin_country")]
    public string? OriginCountry { get; init; }
}

public class SpokenLanguage
{
    [JsonPropertyName("iso_639_1")]
    public string Iso6391 { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("english_name")]
    public string EnglishName { get; init; } = string.Empty;
}

public class Video
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("key")]
    public string Key { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("site")]
    public string Site { get; init; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("official")]
    public bool Official { get; init; }
}

public class VideoResponse
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("results")]
    public List<Video> Results { get; init; } = [];
}
