using Microsoft.AspNetCore.Mvc;
using UrlShortener.Services;

namespace UrlShortener.Controllers
{
    [ApiController]
    [Route("api/{shortCode}")]
    public class UrlRedirectorController : ControllerBase
    {
        private readonly IUrlShortenerService _service;
        public UrlRedirectorController(IUrlShortenerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> RedirectToOriginalUrl(string shortCode, CancellationToken cancellationToken)
        {
            var originalUrl = await _service.GetOriginalUrlAsync(shortCode, cancellationToken);
            if (originalUrl == null)
                return NotFound();

            return RedirectPermanent(originalUrl);
        }
    }
}
