using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Models;

namespace UrlShortener.Repositories
{
    public class ShortUrlRepository : IShortUrlRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ShortUrlRepository> _logger;
        public ShortUrlRepository(ApplicationDbContext context, ILogger<ShortUrlRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(ShortUrl shortUrl, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Adding ShortUrl with ShortCode: {ShortCode}", shortUrl.ShortCode);
            await _context.ShortUrls.AddAsync(shortUrl, cancellationToken);
        }

        public async Task<ShortUrl?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Querying database for ShortCode: {ShortCode}", shortCode);
            return await _context.ShortUrls.FirstOrDefaultAsync(s => s.ShortCode == shortCode, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Saving changes to database");
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
