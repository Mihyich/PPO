using MetroGid.Controllers.DTO;

namespace MetroGid.Controllers.Interfaces;

public interface IClientService
{
    // Создание пользователя в БД
    Task<int> Reg(string login, string password, string mail);
    // Удаление пользователя из БД
    Task<int> UnReg(string login, string password, string mail);
    // Вход
    Task<RoleTypeDTO> SignIn(string login, string password, string mail);
    // Выход
    Task<int> SignOut(string login, string password, string mail);
    // Получить роль пользователя
    Task<RoleTypeDTO> GetRole(string login, string password, string mail);
}