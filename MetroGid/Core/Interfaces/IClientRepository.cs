using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Models.Types;

namespace MetroGid.Core.Interfaces;

public interface IClientRepository
{
    Task<int> AddAsync(Client client);

    Task<int> GetIdAsync(Client client);
    Task<int> GetIdByCredentialsAsync(string login, string password, string mail);
    Task<RoleType> GetRoleByIdAsync(int id);
    Task<Client> GetByIdAsync(int id);
    Task<Client> GetByCredentialsAsync(string login, string password, string mail);

    Task UpdateAsync(int id, Client client);

    Task DeleteAsync(int id);

    Task<bool> IsLoginExistsAsync(string login);
    Task<bool> IsMailExistsAsync(string mail);
}