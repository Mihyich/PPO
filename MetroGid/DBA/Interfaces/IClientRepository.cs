using MetroGid.Core.Models;

namespace MetroGid.DBA.Interfaces
{
    public interface IClientRepository
    {
        Task<int> AddAsync(Client client);

        Task<int> GetIdAsync(Client client);
        Task<Client?> GetByIdAsync(int id);
        Task<Client?> GetByCredentialsAsync(string login, string password, string mail);

        Task UpdateAsync(int id, Client client);

        Task DeleteAsync(int id);

        Task<bool> IsLoginExistsAsync(string login);
        Task<bool> IsMailExistsAsync(string mail);
    }
}