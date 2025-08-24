using MetroGid.Controllers.DTO;
using MetroGid.Controllers.Interfaces;
using MetroGid.Core.Converters;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Interfaces;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utilities.Validators.Handlers;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Classification;

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

    public async Task<int> RegAsync(string login, string password, string mail)
    {
        Client client = new(login, password, mail);
        client.Validate(DomainAttribsValidator);

        await Handler.SnapAsync(
            async () =>
            {
                if (await ClientRepo.IsLoginExistsAsync(login))
                    throw new DataBaseException(
                        $"Логин '{login}' уже занят",
                        ExceptionType.Quiet,
                        ExceptionReason.ItemAlreadyInUse
                    );

                if (await ClientRepo.IsMailExistsAsync(login))
                    throw new DataBaseException(
                        $"Почта '{mail}' уже занята",
                        ExceptionType.Quiet,
                        ExceptionReason.ItemAlreadyInUse
                    );
            }, Logger
        );

        return await ClientRepo.AddAsync(client);
    }

    public async Task<int> UnRegAsync(string login, string password, string mail) =>
        await ClientRepo.DeleteAsync(
            await ClientRepo.GetIdByCredentialsAsync(login, password, mail));

    public async Task<RoleTypeDTO> SignInAsync(string login, string password, string mail) =>
        await GetRoleAsync(login, password, mail);

    public async Task<int> SignOutAsync(string login, string password, string mail) =>
        await ClientRepo.GetIdByCredentialsAsync(login, password, mail);

    public async Task<RoleTypeDTO> GetRoleAsync(string login, string password, string mail)
    {
        int clientId = await Handler.SnapAsync(
            async () =>
            {
                int id = await ClientRepo.GetIdByCredentialsAsync(login, password, mail);

                if (id == 0)
                    throw new DataBaseException(
                        $"Пользователь с логином \"{login}\" и почтой \"{mail}\" не найден",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );

                return id;
            }, Logger
        );

        return DomainDtoConverter.Convert(await ClientRepo.GetRoleByIdAsync(clientId));
    }
}