using FluentValidation;
using UrlShortener.Models;

namespace UrlShortener.Validators
{
    public class ShortenRequestValidator : AbstractValidator<ShortenRequest>
    {
        public ShortenRequestValidator()
        {
            RuleFor(x => x.OriginalUrl)
                .NotEmpty().WithMessage("The URL cannot be empty.")
                .MaximumLength(2048).WithMessage("The URL cannot be more than 2048 characters")
                .Must(BeAValidUrl).WithMessage("The URL must be a valid HTTP or HTTPS URL.");
        }
        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
