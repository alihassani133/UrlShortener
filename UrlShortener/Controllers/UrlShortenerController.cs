using FluentValidation;
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
        private readonly IValidator<ShortenRequest> _validator;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UrlShortenerController> _logger;

        public UrlShortenerController(
            IUrlShortenerService service,
            IConfiguration configuration,
            IValidator<ShortenRequest> validator,
            ILogger<UrlShortenerController> logger)
        {
            _validator = validator;
            _configuration = configuration;
            _service = service;
            _logger = logger;
        }

        [HttpPost("shorten")]
        public async Task<IActionResult> Shorten([FromBody] ShortenRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received shorten request for url: {OriginalUrl}", request.OriginalUrl);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new { Property = e.PropertyName, Error = e.ErrorMessage })
                    .ToList();
                _logger.LogWarning("Validation failed for URL: {OriginalUrl}. Errors: {Errors}",
                    request.OriginalUrl, string.Join(", ", errors.Select(e => $"{e.Property}: {e.Error}")));
                return BadRequest(new {Errors = errors });
            }
            try
            {
                var shortCode = await _service.ShortenUrlAsync(request.OriginalUrl, cancellationToken);
                var scheme = Request.Scheme;
                var host = Request.Host;
                var domain = _configuration["AppSettings:Domain"] ?? $"{scheme}://{host}";
                var shortUrl = $"{domain}/{shortCode}";
                _logger.LogInformation("Generated short URL: {ShortUrl}", shortUrl);
                return Ok(new { ShortUrl = shortUrl });
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Shorten request for URL: {OriginalUrl} was canceled", request.OriginalUrl);
                return StatusCode(499, "Request was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error shortening URL: {OriginalUrl}", request.OriginalUrl);
                return StatusCode(500, "An error occurred while processing your request");
            } 
        }
    }
}
