// Ceviri Servisi

using System.Collections.Frozen;

namespace CinemAI.Services;

public class LanguageService
{
    private static readonly FrozenDictionary<string, FrozenDictionary<string, string>> Translations =
        new Dictionary<string, FrozenDictionary<string, string>>
        {
            ["tr-TR"] = new Dictionary<string, string>
            {
                ["Common.Movies"] = "Filmler",
                ["Common.Search"] = "Ara",
                ["Common.Recommendations"] = "Öneriler",
                ["Common.Popular"] = "Popüler",
                ["Common.Loading"] = "Yükleniyor",
                ["Common.Error"] = "Hata oluştu",
                ["Common.NoResults"] = "Sonuç bulunamadı",

                ["Hero.Title"] = "🎬 CinemAI",
                ["Hero.Subtitle"] = "Kişiselleştirilmiş film önerileri",

                ["Search.Placeholder"] = "🔍 Film ara... (örn: Inception, Matrix)",
                ["Search.Results"] = "Arama Sonuçları",
                ["Search.Back"] = "← Geri",

                ["Section.ForYou"] = "Sizin İçin Öneriler",
                ["Section.Popular"] = "Popüler Filmler",

                ["Movie.AddToList"] = "Listeye Ekle",
                ["Movie.InList"] = "Listede",
                ["Movie.Rate"] = "Puanla",

                ["Cart.Title"] = "Film Listem",
                ["Cart.Empty"] = "Henüz film eklenmedi",
                ["Cart.GetRecommendations"] = "Önerileri Getir",
                ["Cart.Count"] = "film",

                ["Rating.Title"] = "Puanla",
                ["Rating.Question"] = "Bu filmi nasıl buldunuz?",

                ["Message.Added"] = "eklendi",
                ["Message.Removed"] = "Çıkarıldı",

                ["Details.Back"] = "Geri",
                ["Details.Overview"] = "Özet",
                ["Details.Cast"] = "Oyuncular",
                ["Details.Director"] = "Yönetmen",
                ["Details.ReleaseDate"] = "Çıkış Tarihi",
                ["Details.Runtime"] = "Süre",
                ["Details.Genres"] = "Türler",
                ["Details.Rating"] = "Puan",
                ["Details.Minutes"] = "dakika",
                ["Details.Votes"] = "oy",
                ["Details.AddToList"] = "Listeye Ekle",
                ["Details.MovieInfo"] = "Film Bilgileri",
                ["Details.OriginalTitle"] = "Orijinal Adı",
                ["Details.Status"] = "Durum",
                ["Details.StatusReleased"] = "Yayınlandı",
                ["Details.Producer"] = "Yapımcı",
                ["Details.MyList"] = "Listem",
                ["Details.GoToHome"] = "Ana Sayfaya Git",
                ["Details.Unknown"] = "Bilinmiyor",
                ["Details.AlreadyInList"] = "zaten listede!",
                ["Details.AddedWith"] = "ile eklendi!",

                ["Rating.Excellent"] = "Harika",
                ["Rating.Good"] = "İyi",
                ["Rating.Average"] = "Orta",
                ["Rating.Poor"] = "Zayıf",
                ["Rating.Bad"] = "Kötü",

                ["Tag.Genre"] = "Tür",
                ["Tag.Director"] = "Yönetmen",
                ["Tag.Actor"] = "Oyuncu",
            }.ToFrozenDictionary(),

            ["en-US"] = new Dictionary<string, string>
            {
                ["Common.Movies"] = "Movies",
                ["Common.Search"] = "Search",
                ["Common.Recommendations"] = "Recommendations",
                ["Common.Popular"] = "Popular",
                ["Common.Loading"] = "Loading",
                ["Common.Error"] = "An error occurred",
                ["Common.NoResults"] = "No results found",

                ["Hero.Title"] = "🎬 CinemAI",
                ["Hero.Subtitle"] = "Personalized movie recommendations",

                ["Search.Placeholder"] = "🔍 Search movies... (e.g: Inception, Matrix)",
                ["Search.Results"] = "Search Results",
                ["Search.Back"] = "← Back",

                ["Section.ForYou"] = "Recommended For You",
                ["Section.Popular"] = "Popular Movies",

                ["Movie.AddToList"] = "Add to List",
                ["Movie.InList"] = "In List",
                ["Movie.Rate"] = "Rate",

                ["Cart.Title"] = "My Movie List",
                ["Cart.Empty"] = "No movies added yet",
                ["Cart.GetRecommendations"] = "Get Recommendations",
                ["Cart.Count"] = "movies",

                ["Rating.Title"] = "Rate",
                ["Rating.Question"] = "How did you like this movie?",

                ["Message.Added"] = "added",
                ["Message.Removed"] = "Removed",

                ["Details.Back"] = "Back",
                ["Details.Overview"] = "Overview",
                ["Details.Cast"] = "Cast",
                ["Details.Director"] = "Director",
                ["Details.ReleaseDate"] = "Release Date",
                ["Details.Runtime"] = "Runtime",
                ["Details.Genres"] = "Genres",
                ["Details.Rating"] = "Rating",
                ["Details.Minutes"] = "minutes",
                ["Details.Votes"] = "votes",
                ["Details.AddToList"] = "Add to List",
                ["Details.MovieInfo"] = "Movie Info",
                ["Details.OriginalTitle"] = "Original Title",
                ["Details.Status"] = "Status",
                ["Details.StatusReleased"] = "Released",
                ["Details.Producer"] = "Producer",
                ["Details.MyList"] = "My List",
                ["Details.GoToHome"] = "Go to Home",
                ["Details.Unknown"] = "Unknown",
                ["Details.AlreadyInList"] = "already in list!",
                ["Details.AddedWith"] = "added with",

                ["Rating.Excellent"] = "Excellent",
                ["Rating.Good"] = "Good",
                ["Rating.Average"] = "Average",
                ["Rating.Poor"] = "Poor",
                ["Rating.Bad"] = "Bad",

                ["Tag.Genre"] = "Genre",
                ["Tag.Director"] = "Director",
                ["Tag.Actor"] = "Actor",
            }.ToFrozenDictionary()
        }.ToFrozenDictionary();

    public string Get(string key, string language = "tr-TR")
    {
        if (Translations.TryGetValue(language, out var langDict) && langDict.TryGetValue(key, out var translation))
            return translation;

        if (language != "tr-TR" && Translations["tr-TR"].TryGetValue(key, out var fallback))
            return fallback;

        return key;
    }

    public Dictionary<string, string> GetAll(string language = "tr-TR") =>
        Translations.TryGetValue(language, out var dict)
            ? dict.ToDictionary(x => x.Key, x => x.Value)
            : Translations["tr-TR"].ToDictionary(x => x.Key, x => x.Value);
}
