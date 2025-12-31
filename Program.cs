// CinemAI - Film Oneri Sistemi

using CinemAI.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// servis kayitlari

builder.Services.AddControllersWithViews();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});

builder.Services.AddMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// http istemci yapilandirmasi

builder.Services.AddHttpClient<ITmdbApiClient, TmdbApiClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(15);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "CinemAI/1.0");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true,
    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
});

// is mantigi servisleri

builder.Services.AddScoped<IRecommendationEngine, RecommendationEngine>();
builder.Services.AddScoped<ITmdbService, TmdbService>();
builder.Services.AddSingleton<LanguageService>();

// uygulama yapilandirmasi

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.UseResponseCompression();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
