// Resim Proxy Controller

using CinemAI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemAI.Controllers;

[Route("images")]
public class ImageProxyController(ITmdbService tmdbService) : Controller
{
    [HttpGet("{**path}")]
    [ResponseCache(Duration = 86400)]
    public async Task<IActionResult> GetImage(string path)
    {
        if (string.IsNullOrEmpty(path)) return NotFound();

        if (!path.StartsWith('/')) path = "/" + path;

        try
        {
            var stream = await tmdbService.GetImageStreamAsync(path);
            return stream is null ? NotFound() : File(stream, "image/jpeg");
        }
        catch
        {
            return NotFound();
        }
    }
}
