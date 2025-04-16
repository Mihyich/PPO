using System.Net;

namespace MetroGid.Core.Exceptions
{
    public class RegistrationException(RegistrationException.ErrorType reason, string message)
    : DomainException(message, HttpStatusCode.PreconditionFailed)
    {
        public enum ErrorType
        {
            ILLEGAL_LOGIN, // не выполнены правила задания логина
            ILLEGAL_PASSWORD, // не выполнены правила задания пароля
            ILLEGAL_MAIL, // не выполнены правила задания почты
            LOGIN_BUSY, // пользователь с таким логином уже существует
            MAIL_BUSY // пользователь с такой почтой уже существует
        }

        public ErrorType Reason { get; } = reason;
    }
}