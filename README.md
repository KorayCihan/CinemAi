# 🎬 CinemAI - Kişiselleştirilmiş Film Öneri Sistemi

<div align="center">

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-10.0-purple?style=for-the-badge&logo=dotnet)
![TMDB API](https://img.shields.io/badge/TMDB-API-01d277?style=for-the-badge&logo=themoviedatabase)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=for-the-badge&logo=bootstrap)

**Beğendiğiniz filmleri puanlayın, size özel film önerileri alın!**

</div>

---

## 📖 Proje Hakkında

CinemAI, kullanıcıların beğendiği filmleri puanlayarak kişiselleştirilmiş film önerileri almasını sağlayan bir ASP.NET Core MVC uygulamasıdır. Uygulama, TMDB (The Movie Database) API'sini kullanarak güncel film verileri sunar ve içerik tabanlı öneri algoritması ile kullanıcı tercihlerini analiz eder.

## ✨ Özellikler

- 🎯 **Kişiselleştirilmiş Öneriler** - Puanladığınız filmlere göre size özel öneriler
- 🔍 **Film Arama** - TMDB veritabanında anlık film arama
- 🌍 **Çok Dilli Destek** - Türkçe ve İngilizce arayüz
- 📱 **Responsive Tasarım** - Mobil ve masaüstü uyumlu
- ⚡ **Hızlı Performans** - Önbellekleme ve paralel API çağrıları
- 🎭 **Kategori Filtreleme** - Türlere göre film keşfi

## 🛠️ Teknolojiler

| Katman | Teknoloji |
|--------|-----------|
| Backend | ASP.NET Core 10.0 MVC |
| Frontend | Bootstrap 5.3, Vanilla JS |
| API | TMDB (The Movie Database) |
| Caching | IMemoryCache |
| HTTP | HttpClient with Fallback |

## 🚀 Kurulum

### Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- TMDB API Key (opsiyonel - varsayılan key mevcut)

### Adımlar

```bash
# 1. Projeyi klonlayın
git clone https://github.com/KULLANICI_ADINIZ/CinemAI.git

# 2. Proje klasörüne gidin
cd CinemAI

# 3. Uygulamayı çalıştırın
dotnet run
```

Uygulama varsayılan olarak `https://localhost:5001` adresinde çalışacaktır.

### API Key Değiştirme (Opsiyonel)

`appsettings.json` dosyasına kendi TMDB API key'inizi ekleyebilirsiniz:

```json
{
  "TmdbApiKey": "YOUR_API_KEY_HERE"
}
```

## 📁 Proje Yapısı

```
CinemAI/
├── Controllers/
│   ├── HomeController.cs       # Ana sayfa ve öneri işlemleri
│   ├── ImageProxyController.cs # TMDB resim proxy
│   └── LanguageController.cs   # Dil değiştirme
├── Models/
│   ├── Movie.cs                # Film modeli
│   ├── MovieDetails.cs         # Detaylı film bilgisi
│   ├── MovieCredits.cs         # Oyuncu ve ekip
│   └── RecommendationResult.cs # Öneri sonucu
├── Services/
│   ├── TmdbApiClient.cs        # TMDB API istemcisi
│   ├── TmdbService.cs          # Facade servis
│   ├── RecommendationEngine.cs # Öneri motoru
│   └── LanguageService.cs      # Çeviri servisi
├── Views/
│   ├── Home/                   # Ana sayfa view'ları
│   └── Shared/                 # Layout ve partial'lar
└── Program.cs                  # Uygulama başlangıç noktası
```

## 🎯 Kullanım

1. **Ana Sayfa**: Popüler filmler listelenir
2. **Film Seçimi**: Beğendiğiniz filmleri sepete ekleyin
3. **Puanlama**: Her filme 1-5 arası puan verin
4. **Öneri Al**: "Önerileri Getir" butonuna tıklayın
5. **Keşfet**: Size özel film önerileri görüntülenir

## 🔧 Öneri Algoritması

Sistem, içerik tabanlı filtreleme kullanır:

- **Yönetmen Eşleşmesi**: Beğendiğiniz filmlerin yönetmenlerinden filmler
- **Oyuncu Eşleşmesi**: Beğendiğiniz filmlerdeki oyuncuların diğer filmleri
- **Tür Eşleşmesi**: Tercih ettiğiniz türlerden popüler filmler
- **IMDB Weighted Rating**: Kalite puanı hesaplaması

## 📸 Ekran Görüntüleri

> Uygulamayı çalıştırıp ekran görüntüleri ekleyebilirsiniz

## 👥 Katkıda Bulunanlar

- Proje Sahibi

## 📄 Lisans

Bu proje eğitim amaçlı geliştirilmiştir.

---

<div align="center">

**TMDB API ile desteklenmektedir**

<a href="https://www.themoviedb.org/">
  <img src="https://www.themoviedb.org/assets/2/v4/logos/v2/blue_short-8e7b30f73a4020692ccca9c88bafe5dcb6f8a62a4c6bc55cd9ba82bb2cd95f6c.svg" alt="TMDB Logo" width="200">
</a>

*This product uses the TMDB API but is not endorsed or certified by TMDB.*

</div>
