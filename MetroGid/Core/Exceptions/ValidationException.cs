using System.Net;

namespace MetroGid.Core.Exceptions
{
    public class ValidationException(IDictionary<string, string[]> errors) : DomainException("Validation failed", HttpStatusCode.UnprocessableEntity)
    {
        public IDictionary<string, string[]> Errors { get; } = errors;
    }
}