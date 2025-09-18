using MCUD = MetroGid.Controllers.Utility.DTO.Concrete;
using MetroGid.Controllers.Utility.Interfaces;
using MetroGid.Core.Converters;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Interfaces;
using MCMC = MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utility.Validators.Handlers;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Classification;
using MCMT = MetroGid.Core.Models.Types;
using MCMA = MetroGid.Core.Models.Advanced;
using MetroGid.Core.Exceptions.Truistic;
using MetroGid.Core.Models.Advanced;

namespace MetroGid.Core.Services;

public class ClientService(
    IClientRepository clientRepo,
    ThrowableDomainAttribsValidator domainAttribsValidator,
    SuperExceptionHandler handler, IExceptionVisitor? logger = null
) : IClientService
{
    private readonly IClientRepository ClientRepo = clientRepo;

    private readonly ThrowableDomainAttribsValidator DomainAttribsValidator = domainAttribsValidator;
    private readonly SuperExceptionHandler Handler = handler;
    private readonly IExceptionVisitor? Logger = logger;

    public async Task<MCMC.Client> GetClientByIdAsync(int clientId) =>
        await ClientRepo.GetByIdAsync(clientId) ??
            throw new NotFoundByIdException("client", clientId);

    public async Task<MCMA.IdRow> GetClientIdAsync(string login, string password)
    {
        MCMA.IdRow idRow = await ClientRepo.GetIdByCredentialsAsync(login, password);

        if (idRow.id == 0)
            throw new UnknownClientCredentialsException(login, password);

        return idRow;
    }

    public async Task<MCMA.IdRow> RegAsync(string login, string password, string mail)
    {
        MCMC.Client client = new(login, password, mail, MCMT.RoleType.SIGNED);
        client.Validate(DomainAttribsValidator);

        if (await ClientRepo.IsLoginExistsAsync(login))
            throw new LoginInUseException(login);

        if (await ClientRepo.IsMailExistsAsync(mail))
            throw new MailInUseException(mail);

        return await ClientRepo.AddAsync(client);
    }

    public async Task<MCMA.DeletedRowCount> UnRegAsync(int clientId) =>
        await ClientRepo.DeleteAsync(clientId);

    public async Task<MCMC.Client> LogInAsync(string login, string password) =>
        await ClientRepo.GetByCredentialsAsync(login, password) ??
            throw new UnknownClientCredentialsException(login, password);

    public async Task<MCMT.RoleType> GetRoleAsync(string login, string password)
    {
        MCMA.IdRow idRow = await ClientRepo.GetIdByCredentialsAsync(login, password);

        if (idRow.id == 0)
            throw new UnknownClientCredentialsException(login, password);

        return await ClientRepo.GetRoleByIdAsync(idRow.id);
    }

    public async Task<bool> VerifyPasswordAsync(int clientId, string password)
    {
        MCMC.Client client = await GetClientByIdAsync(clientId);
        return client != null ? client.Password == password : false;
    }
}