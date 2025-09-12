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

    public async Task<MCMC.Client?> GetClientByIdAsync(int clientId) =>
        await Handler.SnapAsync(
            async () =>
            {
                MCMC.Client? client = await ClientRepo.GetByIdAsync(clientId);

                if (client == null)
                    throw new DataBaseException(
                        $"Пользователь с айди \"{clientId}\" не найден",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );

                return client;
            }, Logger
        );

    public async Task<MCMA.IdRow> GetClientIdAsync(string login, string password) =>
        await Handler.SnapAsync(
            async () =>
            {
                MCMA.IdRow idRow = await ClientRepo.GetIdByCredentialsAsync(login, password);

                if (idRow.id == 0)
                    throw new DataBaseException(
                        $"Пользователь с логином \"{login}\" не найден",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );

                return idRow;
            }, Logger
        );

    public async Task<MCMA.IdRow> RegAsync(string login, string password, string mail)
    {
        MCMC.Client client = new(login, password, mail, MCMT.RoleType.SIGNED);
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

    public async Task<MCMA.DeletedRowCount> UnRegAsync(int clientId) =>
        await ClientRepo.DeleteAsync(clientId);

    public async Task<MCMC.Client?> LogInAsync(string login, string password)
    {
        MCMC.Client? client = await Handler.SnapAsync(
            async () =>
            {
                MCMC.Client? c = await ClientRepo.GetByCredentialsAsync(login, password);

                if (c == null)
                    throw new DataBaseException(
                        $"Неверный логин '{login}' или пароль '{password}'",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );

                return c;
            }, Logger
        );

        return client;
    }

    public async Task<MCMT.RoleType> GetRoleAsync(string login, string password)
    {
        int clientId = await Handler.SnapAsync(
            async () =>
            {
                MCMA.IdRow idRow = await ClientRepo.GetIdByCredentialsAsync(login, password);

                if (idRow.id == 0)
                    throw new DataBaseException(
                        $"Пользователь с логином \"{login}\" не найден",
                        ExceptionType.Warning,
                        ExceptionReason.NotFound
                    );

                return idRow.id;
            }, Logger
        );

        return await ClientRepo.GetRoleByIdAsync(clientId);
    }

    public async Task<bool> VerifyPasswordAsync(int clientId, string password)
    {
        MCMC.Client? client = await GetClientByIdAsync(clientId);
        return client != null ? client.Password == password : false;
    }
}