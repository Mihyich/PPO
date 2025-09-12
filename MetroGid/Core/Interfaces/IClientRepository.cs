using MCMC = MetroGid.Core.Models.Concrete;
using MCMT = MetroGid.Core.Models.Types;
using MCMA = MetroGid.Core.Models.Advanced;

namespace MetroGid.Core.Interfaces;

public interface IClientRepository
{
    Task<MCMA.IdRow> AddAsync(MCMC.Client client);

    Task<MCMA.IdRow> GetIdAsync(MCMC.Client client);
    Task<MCMA.IdRow> GetIdByCredentialsAsync(string login, string password);

    Task<MCMC.Client?> GetByIdAsync(int id);
    Task<MCMC.Client?> GetByCredentialsAsync(string login, string password);

    Task<MCMT.RoleType> GetRoleByIdAsync(int id);

    Task<MCMA.ChangedRowCount> UpdateAsync(int id, MCMC.Client client);

    Task<MCMA.DeletedRowCount> DeleteAsync(int id);

    Task<bool> IsLoginExistsAsync(string login);
    Task<bool> IsMailExistsAsync(string mail);

    Task<MCMA.IdRow> GetStationDuty(int stationId);
    Task<MCMA.IdRow> GetTransitionDuty(int transitionId);

    Task<MCMA.ChangedRowCount> MakeDutyOfStation(int clientId, int stationId);
    Task<MCMA.ChangedRowCount> MakeDutyOfTransition(int clientId, int transitionId);
    Task<MCMA.ChangedRowCount> MakeDuty(int clientId, int stationId, int transitionId);

    Task<MCMA.ChangedRowCount> DismissDutyFromStation(int stationId);
    Task<MCMA.ChangedRowCount> DismissDutyFromTransition(int transitionId);
    Task<MCMA.ChangedRowCount> DismissDuty(int stationId, int transitionId);
}