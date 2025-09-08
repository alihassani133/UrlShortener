namespace UrlShortener.Models
{
    public class ShortUrl
    {
        public string ShortCode { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;
    }
}
