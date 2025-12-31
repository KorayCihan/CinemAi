// Oneri Sonucu Modeli

namespace CinemAI.Models;

public class RecommendationResult
{
    public required Movie Movie { get; set; }
    public double Score { get; set; }
    public List<string> Reasons { get; set; } = [];

    public string GetReasonsText() => string.Join(" • ", Reasons);
}
