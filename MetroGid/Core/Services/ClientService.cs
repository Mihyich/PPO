using MetroGid.Controllers.DTO;
using MetroGid.Controllers.Interfaces;
using MetroGid.Core.Converters;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utilities.Validators.Handlers;
using MetroGid.DBA.Interfaces;

namespace MetroGid.Core.Services;

public class ClientService(
    IClientRepository clientRepo,
    SuperExceptionHandler handler, IExceptionVisitor? logger = null
) : IClientService
{
    private readonly IClientRepository ClientRepo = clientRepo;

    private readonly SuperExceptionHandler Handler = handler;
    private readonly IExceptionVisitor? Logger = logger;

    private readonly ThrowableDomainAttribsValidator DomainAttribsValidator = new(handler, logger);

    public async Task<int> Reg(string login, string password, string mail)
    {
        Client client = new(login, password, mail);
        client.Validate(DomainAttribsValidator);
        return await ClientRepo.AddAsync(client);
    }

    public async Task UnReg(string login, string password, string mail) =>
        await ClientRepo.DeleteAsync(
            await ClientRepo.GetIdByCredentialsAsync(login, password, mail));

    public async Task<int> SingIn(string login, string password, string mail) =>
        await ClientRepo.GetIdAsync(
            await ClientRepo.GetByCredentialsAsync(login, password, mail));

    public async Task SingOut(string login, string password, string mail) =>
        await ClientRepo.GetByCredentialsAsync(login, password, mail);

    public async Task<RoleTypeDTO> GetRole(string login, string password, string mail) =>
        DomainDtoConverter.Convert((await ClientRepo.GetByCredentialsAsync(login, password, mail)).Role);
}