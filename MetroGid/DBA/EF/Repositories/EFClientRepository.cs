using MetroGid.Core.Interfaces;
using MCMC = MetroGid.Core.Models.Concrete;
using MetroGid.DBA.EF.Models.Tables;
using MetroGid.DBA.EF.Models.UserDefinedTypes;
using MetroGid.DBA.EF.Converters;
using MetroGid.DBA.EF.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MetroGid.DBA.EF.Repositories;

public class EFClientRepository(MetroContext context) : IClientRepository
{
    private readonly MetroContext _context = context;

    public async Task<int> AddAsync(MCMC.Client client)
    {
        Client entity = DomainModelConverter.Convert(client);
        _context.Clients.Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<int> GetIdAsync(MCMC.Client client)
    {
        Client entity = DomainModelConverter.Convert(client);
        
        return await _context.Clients
            .AsNoTracking()
            .Where(c => c.ClientLogin == entity.ClientLogin &&
                    c.ClientPassword == entity.ClientPassword &&
                    c.Mail == entity.Mail)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetIdByCredentialsAsync(string login, string password, string mail)
    {
        return await _context.Clients
            .AsNoTracking()
            .Where(c => c.ClientLogin == login &&
                    c.ClientPassword == password &&
                    c.Mail == mail)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<MCMC.Client> GetByIdAsync(int id)
    {
        Client entity = await _context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new Exception();

        return ModelDomainConverter.Convert(entity);
    }

    public async Task<MCMC.Client> GetByCredentialsAsync(string login, string password, string mail)
    {
        Client entity = await _context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClientLogin == login &&
                c.ClientPassword == password &&
                c.Mail == mail)
            ?? throw new Exception();

        return ModelDomainConverter.Convert(entity);
    }

    public async Task UpdateAsync(int id, MCMC.Client client)
    {
        Client trackEntity = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new Exception();

        Client updatedEntity = DomainModelConverter.Convert(client);

        _context.Entry(trackEntity).CurrentValues.SetValues(updatedEntity);
        
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        Client client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new Exception();

        _context.Clients.Remove(client);
        
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsLoginExistsAsync(string login) =>
        await _context.Clients.AnyAsync(c => c.ClientLogin == login);
    
    public async Task<bool> IsMailExistsAsync(string mail) =>
        await _context.Clients.AnyAsync(c => c.Mail == mail);
}