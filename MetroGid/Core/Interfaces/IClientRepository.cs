using MCMC = MetroGid.Core.Models.Concrete;
using MCMT = MetroGid.Core.Models.Types;

namespace MetroGid.Core.Interfaces;

public interface IClientRepository
{
    Task<int> AddAsync(MCMC.Client client);

    Task<int> GetIdAsync(MCMC.Client client);
    Task<int> GetIdByCredentialsAsync(string login, string password);

    Task<MCMC.Client?> GetByIdAsync(int id);
    Task<MCMC.Client?> GetByCredentialsAsync(string login, string password);

    Task<MCMT.RoleType> GetRoleByIdAsync(int id);

    Task<int> UpdateAsync(int id, MCMC.Client client);

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