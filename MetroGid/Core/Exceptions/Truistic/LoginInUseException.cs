namespace MetroGid.Core.Exceptions.Truistic;

public class LoginInUseException : Exception
{
    public string Login { get; }

    public LoginInUseException(string login)
        : base($"Логин '{login}' уже занят")
    {
        Login = login;
    }
}