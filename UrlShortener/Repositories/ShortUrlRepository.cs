using UrlShortener.Data;
using UrlShortener.Models;

namespace UrlShortener.Repositories
{
    public class ShortUrlRepository : IShortUrlRepository
    {
        private readonly ApplicationDbContext _context;
        public ShortUrlRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ShortUrl shortUrl)
        {
            await _context.ShortUrls.AddAsync(shortUrl);
        }

        public async Task<ShortUrl?> GetByShortCodeAsync(string shortCode)
        {
            return await _context.ShortUrls.FindAsync(shortCode);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
