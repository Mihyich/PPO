namespace MetroGid.Controllers.Interfaces
{
    public interface IClientService
    {
        // Создание пользователя в БД
        Task<int> Reg(string login, string password, string mail);
        // Удаление пользователя из БД
        Task UnReg(string login, string password, string mail);
        // Вход
        Task<int> SingIn(string login, string password, string mail);
        // Выход
        Task SingOut(string login, string password, string mail);
        // Получить роль пользователя
        Task<Client> GetRole(string login, string password, string mail);
    }
}