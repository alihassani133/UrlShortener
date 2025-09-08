# سرویس کوتاه‌کننده لینک با ASP.NET Core

## توضیحات پروژه
این پروژه یک سرویس وب API برای کوتاه کردن لینک‌ها (URL Shortener) است که با استفاده از **ASP.NET Core 8** پیاده‌سازی شده است. هدف آن دریافت یک URL طولانی از کاربر، تولید یک کد کوتاه منحصر به فرد (مانند `aB3xY7`)، و ارائه یک لینک کوتاه (مانند `https://localhost:7213/aB3xY7`) است که کاربر را به URL اصلی هدایت می‌کند.

---
## مشخصات پروژه
- **.NET 8 SDK**: برای اجرای پروژه.
- **SQL Server**: برای ذخیره‌سازی داده‌ها (LocalDB یا SQL Server Express قابل استفاده است).
- **Entity Framework**: برای انجام عملیات CRUD در دیتابیس

---

## معماری پروژه
پروژه از معماری لایه‌ای با رعایت اصول SOLID استفاده می‌کند:
- **لایه Controllers**: درخواست‌های HTTP را مدیریت می‌کند.
  - `UrlShortenerController`: شامل endpoint برای کوتاه کردن لینک (`POST /api/UrlShortener/shorten`).
  - `UrlRedirectorController`: هدایت به URL اصلی (`GET /{shortCode}`).
- **لایه Services**: بیزنس اصلی برنامه مانند تولید کد کوتاه و اعتبارسنجی URL را پیاده‌سازی می‌کند (`IUrlShortenerService`).
- **لایه Repositories**: دسترسی به دیتابیس را مدیریت می‌کند (`IShortUrlRepository`).
- **دیتابیس**: از Entity Framework Core با SQL Server برای ذخیره کدهای کوتاه و URLهای اصلی استفاده شده است.

---

## فرآیند توسعه
1- **راه‌اندازی پروژه**:
   - یک پروژه ASP.NET Core Web API با .NET 8 ایجاد شد.
   - پکیج‌های مورد نیاز با nuget package manager نصب شدند:
   
     ```bash
     Microsoft.EntityFrameworkCore.SqlServer
     Microsoft.EntityFrameworkCore.Tools
     ```

2- **مدل داده**:
   - مدل `ShortUrl` با دو پراپرتی `ShortCode` (کلید اصلی) و `OriginalUrl` تعریف شد.
   - مدل `ShortenRequest` با پراپرتی `OriginalUrl` برای استفاده درر کنترلر `UrlShortenerController` و دریافت لینک اصلی تعریف شد.
   - از `ApplicationDbContext` برای ارتباط با دیتابیس استفاده شد.

3- **لایه Repository**:
   - رابط `IShortUrlRepository` و پیاده‌سازی آن برای عملیات CRUD (ایجاد، خواندن، ذخیره) تعریف شد.
   - این لایه از وابستگی مستقیم به دیتابیس جلوگیری می‌کند.

4- **لایه Service**:
   - سرویس `UrlShortenerService` برای تولید کد کوتاه تصادفی و اعتبارسنجی URL پیاده‌سازی شد.
   - برای تولید کد کوتاه از یک رشته تصادفی 6 کاراکتری استفاده شد که در بخش امتیازی توضیح داده می‌شود.

5- **لایه Controller**:
   - کنترلر `UrlShortenerController` برای endpoint `POST /api/UrlShortener/shorten` ایجاد شد.
   - کنترلر `UrlRedirectorController` برای endpoint `GET /{shortCode}` پیاده‌سازی شد تا هدایت به URL اصلی انجام شود.

6- **مدیریت خطاها**:
   - اعتبارسنجی URL با استفاده از `Uri.TryCreate` انجام شد تا فقط URLهای معتبر پذیرفته شوند.
   - کدهای وضعیت HTTP مناسب (200، 301، 400، 404) برای سناریوهای مختلف پیاده‌سازی شدند.

7- **تست و دیباگ**:
   - از Swagger برای تست اولیه استفاده شد.
   - یک مجموعه Postman (`UrlShortenerAPI.postman_collection.json`) برای تست endpointها ایجاد و در مخزن قرار گرفت.


---

## الگوریتم تولید کد کوتاه (بخش امتیازی)
### روش انتخاب شده
- به جای استفاده از شناسه‌های عددی افزایشی (مانند Auto-increment ID) از یک الگوریتم تولید رشته تصادفی استفاده شد.
- کدهای کوتاه 6 کاراکتری از مجموعه کاراکترهای `a-z`, `A-Z`, `0-9` (62 کاراکتر) تولید می‌شوند.
- کد نمونه در `UrlShortenerService.cs`:
 

  ```csharp
  private async Task<string> GenerateShortCodeAsync(string longUrl)
  {
      const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      var random = new Random();
      string shortCode;
      do
      {
          shortCode = new string(Enumerable.Repeat(Chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
      } while (await _repository.GetByShortCodeAsync(shortCode) != null);
      return shortCode;
  }
  ```

### مزایا
- **غیرقابل پیش‌بینی بودن**: کدها ترتیبی نیستند، بنابراین حدس زدن آن‌ها دشوار است و امنیت بیشتری فراهم می‌کند.
- **سادگی پیاده‌سازی**: نیازی به تبدیل به مبنای خاص نیست.

### معایب
- **احتمال تصادم**: در تعداد زیاد لینک‌ها، احتمال تولید کد تکراری وجود دارد (هرچند با 62^6 ≈ 56 میلیارد امکان، این احتمال کم است).
- **کارایی**: بررسی تصادم در دیتابیس ممکن است در مقیاس بزرگ کند باشد.

### مدیریت تصادم
- اگر کد تولید شده در دیتابیس وجود داشته باشد، یک کد جدید تولید می‌شود تا یکتایی تضمین شود.

---

## مهم:
- مجموعه Postman در فایل `UrlShortenerAPI.postman_collection.json` در پوشه‌ی docs ارائه شده است.
