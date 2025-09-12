using MCMC = MetroGid.Core.Models.Concrete;
using MCMT = MetroGid.Core.Models.Types;
using MCMA = MetroGid.Core.Models.Advanced;

namespace MetroGid.Controllers.Utility.Interfaces;

public interface IClientService
{
    Task<MCMC.Client?> GetClientByIdAsync(int clientId);
    Task<MCMA.IdRow> GetClientIdAsync(string login, string password);
    // Создание пользователя в БД
    Task<MCMA.IdRow> RegAsync(string login, string password, string mail);
    // Удаление пользователя из БД
    Task<MCMA.DeletedRowCount> UnRegAsync(int clientId);
    // Вход
    Task<MCMC.Client?> LogInAsync(string login, string password);
    // Выход
    // Task<int> LogOutAsync(string login, string password);
    // Получить роль пользователя
    Task<MCMT.RoleType> GetRoleAsync(string login, string password);
    // Верификация пароля
    Task<bool> VerifyPasswordAsync(int clientId, string password);
}