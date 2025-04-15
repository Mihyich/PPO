using System.Net;

namespace MetroGid.Services.Exceptions
{
    public class NotFoundException(string entity) : DomainException($"{entity} not found", HttpStatusCode.NotFound) {}
}