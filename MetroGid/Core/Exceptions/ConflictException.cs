using System.Net;

namespace MetroGid.Core.Exceptions
{
    public class ConflictException(string message) : DomainException(message, HttpStatusCode.Conflict) { }
}