using MetroGid.Controllers.Utility.DTO;

namespace MetroGid.Controllers.Utility.Interfaces;

public interface IClientService
{
    Task<ClientDTO?> GetClientByIdAsync(int clientId);
    Task<int> GetClientIdAsync(string login, string password);
    // Создание пользователя в БД
    Task<int> RegAsync(string login, string password, string mail);
    // Удаление пользователя из БД
    Task<int> UnRegAsync(int clientId);
    // Вход
    Task<ClientDTO?> LogInAsync(string login, string password);
    // Выход
    Task<int> LogOutAsync(string login, string password);
    // Получить роль пользователя
    Task<RoleTypeDTO> GetRoleAsync(string login, string password);
    // Верификация пароля
    Task<bool> VerifyPasswordAsync(int clientId, string password);
}