// Kadro ve Ekip Modeli

using System.Text.Json.Serialization;

namespace CinemAI.Models;

public class MovieCredits
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("cast")]
    public List<Cast> Cast { get; init; } = [];

    [JsonPropertyName("crew")]
    public List<Crew> Crew { get; init; } = [];
}

public class Cast
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("character")]
    public string Character { get; init; } = string.Empty;

    [JsonPropertyName("order")]
    public int Order { get; init; }

    [JsonPropertyName("profile_path")]
    public string? ProfilePath { get; init; }
}

public class Crew
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("job")]
    public string Job { get; init; } = string.Empty;

    [JsonPropertyName("department")]
    public string Department { get; init; } = string.Empty;

    [JsonPropertyName("profile_path")]
    public string? ProfilePath { get; init; }
}
