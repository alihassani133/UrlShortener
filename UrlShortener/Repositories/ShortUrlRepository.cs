using Microsoft.EntityFrameworkCore;
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

        public async Task AddAsync(ShortUrl shortUrl, CancellationToken cancellationToken = default)
        {
            await _context.ShortUrls.AddAsync(shortUrl, cancellationToken);
        }

        public async Task<ShortUrl?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default)
        {
            return await _context.ShortUrls.FirstOrDefaultAsync(s => s.ShortCode == shortCode, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
