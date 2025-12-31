// Dil Controller

using Microsoft.AspNetCore.Mvc;

namespace CinemAI.Controllers;

public class LanguageController : Controller
{
    private static readonly string[] SupportedLanguages = ["tr-TR", "en-US"];

    [HttpPost]
    public IActionResult SetLanguage(string lang)
    {
        if (SupportedLanguages.Contains(lang))
        {
            Response.Cookies.Append("UserLanguage", lang, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddYears(1),
                HttpOnly = true,
                SameSite = SameSiteMode.Lax
            });
        }

        return RedirectToAction("Index", "Home");
    }
}
