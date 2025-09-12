namespace UrlShortener.Models
{
    public class ShortUrl
    {
        public int Id { get; set; }
        public string ShortCode { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;
    }
}
