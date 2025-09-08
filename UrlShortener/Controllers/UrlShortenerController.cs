using Microsoft.AspNetCore.Mvc;
using UrlShortener.Models;
using UrlShortener.Services;

namespace UrlShortener.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UrlShortenerController : ControllerBase
    {
        private readonly IUrlShortenerService _service;
        private readonly IConfiguration _configuration;

        public UrlShortenerController(IUrlShortenerService service, IConfiguration configuration)
        {
            _configuration = configuration;
            _service = service;
        }

        [HttpPost("shorten")]
        public async Task<IActionResult> Shorten([FromBody] ShortenRequest request)
        {
            try
            {
                var shortCode = await _service.ShortenUrlAsync(request.OriginalUrl);
                var scheme = Request.Scheme; 
                var host = Request.Host;  
                var domain = _configuration["AppSettings:Domain"] ?? $"{scheme}://{host}";
                var shortUrl = $"{domain}/{shortCode}";
                return Ok(new { ShortUrl = shortUrl });
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid Url");
            }
        }
    }
}
