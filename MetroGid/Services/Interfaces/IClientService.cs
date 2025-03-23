namespace MetroGid.Services.Interfaces
{
    public enum ClientServiceResult
    {
        SUCCESS = 0,
        ILLEGAL_LOGIN, // не выполнены правила задания логина
        ILLEGAL_PASSWORD, // не выполнены правила задания пароля
        ILLEGAL_MAIL, // не выполнены правила задания почты
        LOGIN_BUSY, // пользователь с таким логином уже существует
        MAIL_BUSY, // пользователь с такой почтой уже существует
        INVALID_SING_DATA // неверный логин и/или пароль
    }
    
    public interface IClientService
    {
        // Создание пользователя в БД
        ClientServiceResult Auth(string login, string password, string mail);
        // Вход
        ClientServiceResult Sing_in(string login, string password);
        // Выход
        ClientServiceResult Sing_out(string login, string password);
        // Удаление пользователя из БД
        ClientServiceResult Log_out(string login, string password);
    }
}