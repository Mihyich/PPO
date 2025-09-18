namespace MetroGid.Core.Exceptions.Truistic;

public class UnknownClientCredentialsException : Exception
{
    public string Login { get; }
    public string Password { get; }

    public UnknownClientCredentialsException(string login, string password)
        : base($"Пользователь с логинос '{login}', и паролем '{password}' не найден")
    {
        Login = login;
        Password = password;
    }
}