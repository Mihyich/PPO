using System.Net;

namespace MetroGid.Services.Exceptions
{
    public class ConflictException(string message) : DomainException(message, HttpStatusCode.Conflict) {}
}