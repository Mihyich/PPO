using MetroGid.Controllers.DTO;
using MetroGid.Controllers.Interfaces;
using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
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
                throw new LoginValidationException(
                    "Логин должен содержать 4 - 255 символов",
                    ExceptionType.Quiet,
                    ExceptionReason.ValidationFailed
                );

            if (!Password.IsCorrect(password))
                throw new PasswordValidationException(
                    "Пароль должен содержать 4 - 255 символов",
                    ExceptionType.Quiet,
                    ExceptionReason.ValidationFailed
                );

            if (!Mail.IsCorrect(mail))
                throw new MailValidationException(
                    "Не удалось успешно валидировать почту",
                    ExceptionType.Quiet,
                    ExceptionReason.ValidationFailed
                );

            if (await ClientRepo.IsLoginExistsAsync(login))
                throw new ItemAlreadyInUseException(
                    $"Логин '{login}' уже занят",
                    ExceptionType.Quiet,
                    ExceptionReason.ItemAlreadyInUse
                );

            if (await ClientRepo.IsMailExistsAsync(mail))
                throw new ItemAlreadyInUseException(
                    $"Почта '{mail}' уже занята",
                    ExceptionType.Quiet,
                    ExceptionReason.ItemAlreadyInUse
                );

            Client client = new(login, password, mail);
            int id = await ClientRepo.AddAsync(client);
            return id;
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
            CntRoleType.Convert((await ClientRepo.GetByCredentialsAsync(login, password, mail)).Role);
    }
}