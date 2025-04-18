using MetroGid.Controllers.DTO;
using MetroGid.Controllers.Interfaces;
using MetroGid.Core.Exceptions;
using MetroGid.Core.Models;
using MetroGid.Core.Utilities;
using MetroGid.DBA.Interfaces; 

namespace MetroGid.Core.Services
{
    public class ClientService(IClientRepository clientRepo) : IClientService
    {
        private readonly IClientRepository ClientRepo = clientRepo;

        public async Task<int> Reg(string login, string password, string mail)
        {
            if (!Login.IsCorrect(login))
                throw new RegistrationException(
                    RegistrationException.ErrorType.ILLEGAL_LOGIN,
                    "Логин должен содержать 4 - 255 символов"
                );

            if (!Password.IsCorrect(password))
                throw new RegistrationException(
                    RegistrationException.ErrorType.ILLEGAL_PASSWORD,
                    "Пароль должен содержать 4 - 255 символов"
                );

            if (!Mail.IsCorrect(mail))
                throw new RegistrationException(
                    RegistrationException.ErrorType.ILLEGAL_MAIL,
                    "Не удалось успешно валидировать почту"
                );

            if (await ClientRepo.IsLoginExistsAsync(login))
                throw new RegistrationException(
                    RegistrationException.ErrorType.LOGIN_BUSY,
                    $"Логин '{login}' уже занят"
                );

            if (await ClientRepo.IsMailExistsAsync(mail))
                throw new RegistrationException(
                    RegistrationException.ErrorType.MAIL_BUSY,
                    $"Почта '{mail}' уже занята"
                );

            Client client = new(login, password, mail);
            int id = await ClientRepo.AddAsync(client);

            return id;
        }

        public async Task UnReg(string login, string password, string mail)
        {
            Client? client = await ClientRepo.GetByCredentialsAsync(login, password, mail) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.ClientLost,
                    $"Пользователь не найден:\n  Логин: '{login}'\n  Пароль: '{password}'\n  Почта: '{mail}'"
                );

            int id = await ClientRepo.GetIdAsync(client);
            await ClientRepo.DeleteAsync(id);
        }

        public async Task<int> SingIn(string login, string password, string mail)
        {
            Client? client = await ClientRepo.GetByCredentialsAsync(login, password, mail) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.ClientLost,
                    $"Пользователь не найден:\n  Логин: '{login}'\n  Пароль: '{password}'\n  Почта: '{mail}'"
                );

            return await ClientRepo.GetIdAsync(client);
        }

        public async Task SingOut(string login, string password, string mail)
        {
            _ = await ClientRepo.GetByCredentialsAsync(login, password, mail) ??
                throw new NotFoundException(
                    NotFoundException.ErrorType.ClientLost,
                    $"Пользователь не найден:\n  Логин: '{login}'\n  Пароль: '{password}'\n  Почта: '{mail}'"
                );
        }

        public async Task<RoleTypeDTO> GetRole(string login, string password, string mail)
        {
            Client? client = await ClientRepo.GetByCredentialsAsync(login, password, mail);
            return client != null ? CntRoleTypeDTO.Convert(client.Role) : RoleTypeDTO.UNSIGNED;
        }
    }
}