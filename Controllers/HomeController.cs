// Ana Controller

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using CinemAI.Models;
using CinemAI.Services;

namespace CinemAI.Controllers;

public class HomeController(
    ITmdbService tmdbService,
    ILogger<HomeController> logger,
    IMemoryCache cache,
    LanguageService languageService) : Controller
{
    private const string CacheKeyPrefix = "PopularMovies";

    private string CurrentLanguage => Request.Cookies["UserLanguage"] ?? "tr-TR";

    // sayfa aksiyonlari

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var language = CurrentLanguage;
        SetViewBagLanguage(language);

        var viewModel = new RatingViewModel();

        try
        {
            viewModel.Movies = await GetCachedPopularMoviesAsync(language);

            if (viewModel.Movies.Count == 0)
            {
                ViewBag.Error = languageService.Get("Common.Error", language);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Popüler filmler yüklenirken hata oluştu");
            ViewBag.Error = languageService.Get("Common.Error", language);
        }

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var language = CurrentLanguage;
        SetViewBagLanguage(language);

        try
        {
            var movieDetails = await tmdbService.GetMovieDetailsAsync(id, language);

            if (movieDetails is null)
            {
                logger.LogWarning("Film bulunamadı: {MovieId}", id);
                return RedirectToAction("Index");
            }

            return View(movieDetails);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Film detayları yüklenirken hata: {MovieId}", id);
            return RedirectToAction("Index");
        }
    }

    // oneri sistemi

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Recommend(Dictionary<int, int> ratings)
    {
        var language = CurrentLanguage;
        SetViewBagLanguage(language);

        var viewModel = new RatingViewModel();

        try
        {
            viewModel.Movies = await GetCachedPopularMoviesAsync(language);

            var validRatings = ratings?
                .Where(r => r.Value > 0)
                .ToDictionary(r => r.Key, r => r.Value)
                ?? [];

            if (validRatings.Count == 0)
            {
                ViewBag.Warning = "Lütfen en az bir filme puan verin.";
                return View("Index", viewModel);
            }

            viewModel.RecommendedMovies = await tmdbService.GetEnhancedRecommendationsAsync(
                validRatings, viewModel.Movies, language);

            viewModel.ParseReasons();

            logger.LogInformation(
                "Kullanıcı {RatingCount} film puanladı. {RecommendationCount} öneri gösterildi.",
                validRatings.Count, viewModel.RecommendedMovies.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Öneri hesaplanırken hata oluştu");
            ViewBag.Error = "Öneriler hesaplanırken bir hata oluştu.";
        }

        return View("Index", viewModel);
    }

    // ajax endpointler

    [HttpGet]
    public async Task<IActionResult> Search(string q)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return Json(new { success = false, message = "En az 2 karakter girin" });

        try
        {
            var movies = await tmdbService.SearchMoviesAsync(q, CurrentLanguage);

            var results = movies
                .Select(m => new
                {
                    id = m.Id,
                    title = m.Title,
                    posterUrl = m.GetPosterUrl(),
                    voteAverage = m.VoteAverage,
                    genreIds = m.GenreIds
                })
                .Take(20)
                .ToList();

            return Json(new { success = true, movies = results });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Film arama hatası: {Query}", q);
            return Json(new { success = false, message = "Arama sırasında bir hata oluştu" });
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();

    // yardimci metotlar

    private void SetViewBagLanguage(string language)
    {
        ViewBag.Language = language;
        ViewBag.Translations = languageService.GetAll(language);
    }

    private async Task<List<Movie>> GetCachedPopularMoviesAsync(string language)
    {
        var cacheKey = $"{CacheKeyPrefix}_{language}";

        if (!cache.TryGetValue(cacheKey, out List<Movie>? movies))
        {
            movies = await tmdbService.GetPopularMoviesAsync(language);
            cache.Set(cacheKey, movies, TimeSpan.FromMinutes(10));
        }

        return movies ?? [];
    }
}
