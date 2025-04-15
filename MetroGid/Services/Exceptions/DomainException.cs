using System.Net;

namespace MetroGid.Services.Exceptions
{
    public abstract class DomainException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest) : Exception(message)
    {
        public HttpStatusCode StatusCode { get; } = statusCode;
    }
}