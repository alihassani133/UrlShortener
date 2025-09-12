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

        public UrlShortenerController(IUrlShortenerService service, IConfiguration configuration, IValidator<ShortenRequest> validator)
        {
            _validator = validator;
            _configuration = configuration;
            _service = service;
        }

        [HttpPost("shorten")]
        public async Task<IActionResult> Shorten([FromBody] ShortenRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new { Property = e.PropertyName, Error = e.ErrorMessage })
                    .ToList();
                return BadRequest(new {Errors = errors });
            }

            var shortCode = await _service.ShortenUrlAsync(request.OriginalUrl, cancellationToken);
            var scheme = Request.Scheme;
            var host = Request.Host;
            var domain = _configuration["AppSettings:Domain"] ?? $"{scheme}://{host}";
            var shortUrl = $"{domain}/{shortCode}";
            return Ok(new { ShortUrl = shortUrl });
        }
    }
}
