using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Models.Types;

namespace MetroGid.Core.Interfaces;

public interface IClientRepository
{
    Task<int> AddAsync(Client client);

    Task<int> GetIdAsync(Client client);
    Task<int> GetIdByCredentialsAsync(string login, string password);

    Task<Client?> GetByIdAsync(int id);
    Task<Client?> GetByCredentialsAsync(string login, string password);

    Task<RoleType> GetRoleByIdAsync(int id);

    Task<int> UpdateAsync(int id, Client client);

    Task<int> DeleteAsync(int id);

    Task<bool> IsLoginExistsAsync(string login);
    Task<bool> IsMailExistsAsync(string mail);

    Task<int> GetStationDuty(int stationId);
    Task<int> GetTransitionDuty(int transitionId);

    Task<int> MakeDutyOfStation(int clientId, int stationId);
    Task<int> MakeDutyOfTransition(int clientId, int transitionId);
    Task<int> MakeDuty(int clientId, int stationId, int transitionId);

    Task<int> DismissDutyFromStation(int stationId);
    Task<int> DismissDutyFromTransition(int transitionId);
    Task<int> DismissDuty(int stationId, int transitionId);
}