using UrlShortener.Models;

namespace UrlShortener.Repositories
{
    public interface IShortUrlRepository
    {
        Task<ShortUrl?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default);
        Task AddAsync(ShortUrl shortUrl, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
