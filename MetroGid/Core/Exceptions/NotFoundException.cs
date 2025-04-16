using System.Net;

namespace MetroGid.Core.Exceptions
{
    public class NotFoundException(NotFoundException.ErrorType reason, string message) : DomainException(message, HttpStatusCode.NotFound)
    {
        public enum ErrorType
        {
            ClientLost // Пользователь не найден
        }

        public ErrorType Reason { get; } = reason;
    }
}