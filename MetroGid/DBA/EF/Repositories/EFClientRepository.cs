using MetroGid.Core.Interfaces;
using MCMC = MetroGid.Core.Models.Concrete;
using MCMT = MetroGid.Core.Models.Types;
using MCMA = MetroGid.Core.Models.Advanced;
using MDEMT = MetroGid.DBA.EF.Models.Tables;
using MetroGid.DBA.EF.Converters;
using MetroGid.DBA.EF.Context;
using Microsoft.EntityFrameworkCore;

namespace MetroGid.DBA.EF.Repositories;

public class EFClientRepository(MetroDbContext context) : IClientRepository
{
    private readonly MetroDbContext _context = context;

    public async Task<MCMA.IdRow> AddAsync(MCMC.Client client)
    {
        MDEMT.Client entity = DomainModelConverter.Convert(client);
        await _context.Clients.AddAsync(entity);
        await _context.SaveChangesAsync();
        return new(entity.Id);
    }

    public async Task<MCMA.IdRow> GetIdAsync(MCMC.Client client)
    {
        MDEMT.Client entity = DomainModelConverter.Convert(client);
        return await GetIdByCredentialsAsync(entity.ClientLogin, entity.ClientPassword);
    }

    public async Task<MCMA.IdRow> GetIdByCredentialsAsync(string login, string password) =>
        new (await _context.Clients
            .AsNoTracking()
            .Where(c => c.ClientLogin == login &&
                    c.ClientPassword == password)
            .Select(c => c.Id)
            .FirstOrDefaultAsync()
        );

    public async Task<MCMC.Client?> GetByIdAsync(int id)
    {
        MDEMT.Client? entity = await _context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        return entity != null ? ModelDomainConverter.Convert(entity) : null;
    }

    public async Task<MCMC.Client?> GetByCredentialsAsync(string login, string password)
    {
        MDEMT.Client? entity = await _context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClientLogin == login &&
                c.ClientPassword == password);

        return entity != null ? ModelDomainConverter.Convert(entity) : null;
    }

    public async Task<MCMT.RoleType> GetRoleByIdAsync(int id) =>
        ModelDomainConverter.Convert<MCMT.RoleType>(
            await _context.Clients
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => c.Privilege)
                .FirstOrDefaultAsync() ??
                MCMT.RoleType.UNSIGNED.ToString()
        );

    public async Task<MCMA.ChangedRowCount> UpdateAsync(int id, MCMC.Client client)
    {
        MDEMT.Client updClient = DomainModelConverter.Convert(client);

        return new(await _context.Clients
            .Where(c => c.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(c => c.ClientLogin, updClient.ClientLogin)
                .SetProperty(c => c.ClientPassword, updClient.ClientPassword)
                .SetProperty(c => c.Mail, updClient.Mail)
                .SetProperty(c => c.Privilege, updClient.Privilege)
            )
        );
    }

    public async Task<MCMA.DeletedRowCount> DeleteAsync(int id) =>
        new(await _context.Clients
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync()
        );

    public async Task<bool> IsLoginExistsAsync(string login) =>
        await _context.Clients.AsNoTracking().AnyAsync(c => c.ClientLogin == login);
    
    public async Task<bool> IsMailExistsAsync(string mail) =>
        await _context.Clients.AsNoTracking().AnyAsync(c => c.Mail == mail);

    public async Task<MCMA.IdRow> GetStationDuty(int stationId) =>
        new (await _context.Stations
            .AsNoTracking()
            .Where(s => s.Id == stationId)
            .Select(s => s.DutyId)
            .FirstOrDefaultAsync()
            ?? 0
        );

    public async Task<MCMA.IdRow> GetTransitionDuty(int transitionId) =>
        new(await _context.Transitions
            .AsNoTracking()
            .Where(t => t.Id == transitionId)
            .Select(t => t.DutyId)
            .FirstOrDefaultAsync()
            ?? 0
        );

    public async Task<MCMA.ChangedRowCount> MakeDutyOfStation(int clientId, int stationId) =>
        new (await _context.Stations
            .Where(s => s.Id == stationId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(s => s.DutyId, clientId))
        );

    public async Task<MCMA.ChangedRowCount> MakeDutyOfTransition(int clientId, int transitionId) =>
        new (await _context.Transitions
            .Where(t => t.Id == transitionId)
            .ExecuteUpdateAsync(t => t
                .SetProperty(t => t.DutyId, clientId))
        );

    public async Task<MCMA.ChangedRowCount> MakeDuty(int clientId, int stationId, int transitionId)
    {
        MCMA.ChangedRowCount dutyStationRowCount = await MakeDutyOfStation(clientId, stationId);
        MCMA.ChangedRowCount dutyTransitionRowCount = await MakeDutyOfTransition(clientId, transitionId);
        return new(dutyStationRowCount.count + dutyTransitionRowCount.count);
    }

    public async Task<MCMA.ChangedRowCount> DismissDutyFromStation(int stationId) =>
        new (await _context.Stations
            .Where(s => s.Id == stationId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(s => s.DutyId, (int?)null))
        );

    public async Task<MCMA.ChangedRowCount> DismissDutyFromTransition(int transitionId) =>
        new (await _context.Transitions
                .Where(t => t.Id == transitionId)
                .ExecuteUpdateAsync(t => t
                    .SetProperty(t => t.DutyId, (int?)null))
        );

    public async Task<MCMA.ChangedRowCount> DismissDuty(int stationId, int transitionId)
    {
        MCMA.ChangedRowCount dismissStationRowCount = await DismissDutyFromStation(stationId);
        MCMA.ChangedRowCount dismissTransitionRowCount = await DismissDutyFromTransition(transitionId);
        return new(dismissStationRowCount.count + dismissTransitionRowCount.count);
    }
}