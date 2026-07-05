# EndlessTime - Zaman ve GÃ¶rev Takip Sistemi

EndlessTime, projelerdeki Ã§alÄ±ÅŸma sÃ¼relerini kaydetmek, gÃ¶revleri yÃ¶netmek ve Ã¼retkenliÄŸi artÄ±rmak amacÄ±yla geliÅŸtirilmiÅŸ Ã§ok katmanlÄ± bir web uygulamasÄ±dÄ±r.

## ğŸš€ KullanÄ±lan Teknolojiler
* **Mimari:** ASP.NET Core MVC (Ã‡ok KatmanlÄ± Mimari)
* **Katmanlar:**
  * `EndlessTime` (ArayÃ¼z ve Denetleyici KatmanÄ±)
  * `EndlessTime.Data` (Veri EriÅŸim ve Repository KatmanÄ±)
  * `EndlessTime.Model` (VarlÄ±k ve Veri Modelleri KatmanÄ±)
* **TasarÄ±m:** HTML, CSS, JavaScript, Bootstrap, Areas (Admin/User ayrÄ±mÄ±)

## âœ¨ Ã–zellikler / YapÄ±
* Ã‡ok katmanlÄ± mimari (N-Tier Architecture) yapÄ±sÄ± ile sÃ¼rdÃ¼rÃ¼lebilir kod tabanÄ±.
* Admin ve KullanÄ±cÄ± rolleri iÃ§in `Areas` modÃ¼lÃ¼ ile ayrÄ±lmÄ±ÅŸ kontrol panelleri.
* GÃ¶rev atama, zaman Ã§izelgesi (timesheet) kaydetme ve Ã§alÄ±ÅŸma raporlarÄ±.

## ğŸ› ï¸ NasÄ±l Ã‡alÄ±ÅŸtÄ±rÄ±lÄ±r?
1. `appsettings.json` dosyasÄ±ndan veritabanÄ± baÄŸlantÄ± dizesini gÃ¼ncelleyin.
2. EF Core Migrations veya SQL Script yardÄ±mÄ±yla veritabanÄ±nÄ± oluÅŸturun.
3. Projeyi Ã§alÄ±ÅŸtÄ±rÄ±p tarayÄ±cÄ±dan test edin.