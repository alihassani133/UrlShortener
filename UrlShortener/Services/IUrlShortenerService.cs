namespace UrlShortener.Services
{
    public interface IUrlShortenerService
    {
        Task<string> ShortenUrlAsync(string originalUrl, CancellationToken cancellationToken = default);
        Task<string?> GetOriginalUrlAsync(string shortCode, CancellationToken cancellationToken = default);
    }
}
