
using UrlShortener.Models;
using UrlShortener.Repositories;

namespace UrlShortener.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly IShortUrlRepository _repository;
        private const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private readonly Random _random = new Random();
        private const int CodeLength = 6;
        public UrlShortenerService(IShortUrlRepository repository)
        {
            _repository = repository;
        }
        public async Task<string?> GetOriginalUrlAsync(string shortCode, CancellationToken cancellationToken = default)
        {
            var shortUrl = await _repository.GetByShortCodeAsync(shortCode, cancellationToken);
            return shortUrl?.OriginalUrl;
        }

        public async Task<string> ShortenUrlAsync(string originalUrl, CancellationToken cancellationToken = default)
        {
            string shortCode;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                shortCode = new string(Enumerable.Repeat(Chars, CodeLength)
                    .Select(s => s[_random.Next(s.Length)]).ToArray());
            }
            while (await _repository.GetByShortCodeAsync(shortCode, cancellationToken) != null);

            var shortUtl = new ShortUrl { ShortCode = shortCode, OriginalUrl = originalUrl };

            await _repository.AddAsync(shortUtl, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return shortCode;
        }
    }
}
