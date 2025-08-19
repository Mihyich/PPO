using MetroGid.Core.Interfaces;
using MCMC = MetroGid.Core.Models.Concrete;
using MCMT = MetroGid.Core.Models.Types;
using MDEMT = MetroGid.DBA.EF.Models.Tables;
using MetroGid.DBA.EF.Converters;
using MetroGid.DBA.EF.Context;
using Microsoft.EntityFrameworkCore;

namespace MetroGid.DBA.EF.Repositories;

public class EFClientRepository(MetroDbContext context) : IClientRepository
{
    private readonly MetroDbContext _context = context;

    public async Task<int> AddAsync(MCMC.Client client)
    {
        MDEMT.Client entity = DomainModelConverter.Convert(client);
        await _context.Clients.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<int> GetIdAsync(MCMC.Client client)
    {
        MDEMT.Client entity = DomainModelConverter.Convert(client);
        return await GetIdByCredentialsAsync(entity.ClientLogin, entity.ClientPassword, entity.Mail);
    }

    public async Task<int> GetIdByCredentialsAsync(string login, string password, string mail) =>
        await _context.Clients
            .AsNoTracking()
            .Where(c => c.ClientLogin == login &&
                    c.ClientPassword == password &&
                    c.Mail == mail)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();

    public async Task<MCMT.RoleType> GetRoleByIdAsync(int id) =>
        ModelDomainConverter.Convert<MCMT.RoleType>(
            await _context.Clients
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => c.Privilege)
                .FirstOrDefaultAsync() ??
                MCMT.RoleType.UNSIGNED.ToString()
        );

    public async Task<MCMC.Client?> GetByIdAsync(int id)
    {
        MDEMT.Client? entity = await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
        return entity != null ? ModelDomainConverter.Convert(entity) : null;
    }

    public async Task<MCMC.Client?> GetByCredentialsAsync(string login, string password, string mail)
    {
        MDEMT.Client? entity = await _context.Clients
            .FirstOrDefaultAsync(c => c.ClientLogin == login &&
                c.ClientPassword == password &&
                c.Mail == mail);

        return entity != null ? ModelDomainConverter.Convert(entity) : null;
    }

    public async Task<int> UpdateAsync(int id, MCMC.Client client)
    {
        MDEMT.Client updClient = DomainModelConverter.Convert(client);

        return await _context.Clients
            .Where(c => c.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(c => c.ClientLogin, updClient.ClientLogin)
                .SetProperty(c => c.ClientPassword, updClient.ClientPassword)
                .SetProperty(c => c.Mail, updClient.Mail)
                .SetProperty(c => c.Privilege, updClient.Privilege)
            );
    }

    public async Task<int> DeleteAsync(int id) =>
        await _context.Clients
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();

    public async Task<bool> IsLoginExistsAsync(string login) =>
        await _context.Clients.AnyAsync(c => c.ClientLogin == login);
    
    public async Task<bool> IsMailExistsAsync(string mail) =>
        await _context.Clients.AnyAsync(c => c.Mail == mail);

    public async Task<int> GetStationDuty(int stationId) =>
        await _context.Stations
            .AsNoTracking()
            .Where(s => s.Id == stationId)
            .Select(s => s.DutyId)
            .FirstOrDefaultAsync()
            ?? 0;

    public async Task<int> GetTransitionDuty(int transitionId) =>
        await _context.Transitions
            .AsNoTracking()
            .Where(t => t.Id == transitionId)
            .Select(t => t.DutyId)
            .FirstOrDefaultAsync()
            ?? 0;

    public async Task<int> MakeDutyOfStation(int clientId, int stationId) =>
        await _context.Stations
            .Where(s => s.Id == stationId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(s => s.DutyId, clientId));

    public async Task<int> MakeDutyOfTransition(int clientId, int transitionId) =>
        await _context.Transitions
            .Where(t => t.Id == transitionId)
            .ExecuteUpdateAsync(t => t
                .SetProperty(t => t.DutyId, clientId));

    public async Task<int> MakeDuty(int clientId, int stationId, int transitionId) =>
        await MakeDutyOfStation(clientId, stationId) +
        await MakeDutyOfTransition(clientId, transitionId);

    public async Task<int> DismissDutyFromStation(int stationId) =>
        await _context.Stations
            .Where(s => s.Id == stationId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(s => s.DutyId, (int?)null));

    public async Task<int> DismissDutyFromTransition(int transitionId) =>
        await _context.Transitions
                .Where(t => t.Id == transitionId)
                .ExecuteUpdateAsync(t => t
                    .SetProperty(t => t.DutyId, (int?)null));

    public async Task<int> DismissDuty(int stationId, int transitionId) =>
        await DismissDutyFromStation(stationId) +
        await DismissDutyFromTransition(transitionId);
}