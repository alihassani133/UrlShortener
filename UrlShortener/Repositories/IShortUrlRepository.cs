using UrlShortener.Models;

namespace UrlShortener.Repositories
{
    public interface IShortUrlRepository
    {
        Task<ShortUrl?> GetByShortCodeAsync(string shortCode);
        Task AddAsync(ShortUrl shortUrl);
        Task SaveChangesAsync();
    }
}
