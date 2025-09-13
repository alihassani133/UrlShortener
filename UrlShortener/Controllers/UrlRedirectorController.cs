using Microsoft.AspNetCore.Mvc;
using UrlShortener.Services;

namespace UrlShortener.Controllers
{
    [ApiController]
    [Route("api/{shortCode}")]
    public class UrlRedirectorController : ControllerBase
    {
        private readonly IUrlShortenerService _service;
        private readonly ILogger<UrlRedirectorController> _logger;
        public UrlRedirectorController(IUrlShortenerService service, ILogger<UrlRedirectorController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> RedirectToOriginalUrl(string shortCode, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received redirect request for ShortCode: {ShortCode}", shortCode);
            try
            {
                var originalUrl = await _service.GetOriginalUrlAsync(shortCode, cancellationToken);
                if (originalUrl == null)
                {
                    _logger.LogWarning("ShortCode not found: {ShortCode}", shortCode);
                    return NotFound();
                }

                _logger.LogInformation("Redirecting to {OriginalUrl}", originalUrl);
                return RedirectPermanent(originalUrl);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Redirect request for ShortCode: {ShortCode} was canceled", shortCode);
                return StatusCode(499, "Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error redirecting for ShortCode: {ShortCode}", shortCode);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
