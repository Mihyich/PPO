using MetroGid.Controllers.Utility.DTO;

namespace MetroGid.Controllers.Utility.Interfaces;

public interface IClientService
{
    // Создание пользователя в БД
    Task<int> RegAsync(string login, string password, string mail);
    // Удаление пользователя из БД
    Task<int> UnRegAsync(string login, string password, string mail);
    // Вход
    Task<RoleTypeDTO> SignInAsync(string login, string password, string mail);
    // Выход
    Task<int> SignOutAsync(string login, string password, string mail);
    // Получить роль пользователя
    Task<RoleTypeDTO> GetRoleAsync(string login, string password, string mail);
}