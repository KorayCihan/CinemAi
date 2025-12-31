// Tur Modeli

using System.Text.Json.Serialization;

namespace CinemAI.Models;

public class Genre
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}
