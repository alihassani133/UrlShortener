
using UrlShortener.Models;
using UrlShortener.Repositories;

namespace UrlShortener.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly IShortUrlRepository _repository;
        private readonly ILogger<UrlShortenerService> _logger;
        private const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private readonly Random _random = new Random();
        private const int CodeLength = 6;
        public UrlShortenerService(IShortUrlRepository repository, ILogger<UrlShortenerService> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public async Task<string?> GetOriginalUrlAsync(string shortCode, CancellationToken cancellationToken = default)
        {
            await Task.Delay(5000, cancellationToken);
            _logger.LogInformation("Retreiving OriginalUrl for ShortCode: {ShortCode}", shortCode);
            var shortUrl = await _repository.GetByShortCodeAsync(shortCode, cancellationToken);
            if (shortUrl == null)
                _logger.LogWarning("No OriginalUrl found for ShortCode: {ShortCode}", shortCode);
            return shortUrl?.OriginalUrl;
        }

        public async Task<string> ShortenUrlAsync(string originalUrl, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting short code generation for URL: {OriginalUrl}", originalUrl);

            string shortCode;
            int attempts = 0;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                shortCode = new string(Enumerable.Repeat(Chars, CodeLength)
                    .Select(s => s[_random.Next(s.Length)]).ToArray());
                attempts++;
                _logger.LogDebug("Generated ShortCode: {ShortCode}, attempt: {Attempt}", shortCode, attempts);
            }
            while (await _repository.GetByShortCodeAsync(shortCode, cancellationToken) != null);

            var shortUtl = new ShortUrl { ShortCode = shortCode, OriginalUrl = originalUrl };

            await _repository.AddAsync(shortUtl, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully generated ShortCode: {ShortCode} for URL: {OriginalUrl}", shortCode, originalUrl);
            return shortCode;
        }
    }
}
