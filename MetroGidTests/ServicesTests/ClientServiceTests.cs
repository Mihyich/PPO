using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Interfaces;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Services;
using Moq;

namespace MetroGidTests.ServicesTests;

public class ClientServiceTests
{
    [Theory]
    [InlineData("abc", "1234", "abc@mail.ru", "Логин клиента с почтой 'abc@mail.ru' имеет недопустимую длину: 3 не принадлежит [4, 255]", ExceptionType.Warning, ExceptionReason.StringLenghtOutOfRange)]
    [InlineData("abcd", "123", "abc@mail.ru", "Пароль клиента 'abcd' с почтой 'abc@mail.ru' имеет недопустимую длину: 3 не принадлежит [4, 255]", ExceptionType.Warning, ExceptionReason.StringLenghtOutOfRange)]
    public async void ClientAttribsRegTest(string login, string password, string mail, string exMessege, ExceptionType exType, ExceptionReason exReason)
    {
        Mock<IClientRepository> mockClientRepo = new();
        SuperHandlerException handler = new PassThroughHandlerException();
        ClientService clientService = new(mockClientRepo.Object, handler);

        mockClientRepo.Setup(x => x.AddAsync(It.IsAny<Client>())).ReturnsAsync(1);
        var ex = await Assert.ThrowsAsync<DomainValidationException>(async () => { await clientService.Reg(login, password, mail); });

        mockClientRepo.Verify(x => x.AddAsync(It.IsAny<Client>()), Times.Never);
        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }

    [Fact]
    public async void ClientLoginInUseRegTest()
    {
        string login = "abcd";
        string password = "1234";
        string mail = "abc@mail.ru";

        string exMessege = $"Логин '{login}' уже занят";
        ExceptionType exType = ExceptionType.Quiet;
        ExceptionReason exReason = ExceptionReason.ItemAlreadyInUse;

        DataBaseException expectedEx = new(exMessege, exType, exReason);

        Mock<IClientRepository> mockClientRepo = new();
        SuperHandlerException handler = new PassThroughHandlerException();
        ClientService clientService = new(mockClientRepo.Object, handler);

        mockClientRepo.Setup(x => x.AddAsync(It.IsAny<Client>())).ThrowsAsync(expectedEx);
        var ex = await Assert.ThrowsAsync<DataBaseException>(async () => { await clientService.Reg(login, password, mail); });

        mockClientRepo.Verify(x => x.AddAsync(It.IsAny<Client>()), Times.Once);
        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }

    [Fact]
    public async void ClientMailInUseRegTest()
    {
        string login = "abcd";
        string password = "1234";
        string mail = "abc@mail.ru";

        string exMessege = $"Почта '{mail}' уже занята";
        ExceptionType exType = ExceptionType.Quiet;
        ExceptionReason exReason = ExceptionReason.ItemAlreadyInUse;

        DataBaseException expectedEx = new(exMessege, exType, exReason);

        Mock<IClientRepository> mockClientRepo = new();
        SuperHandlerException handler = new PassThroughHandlerException();
        ClientService clientService = new(mockClientRepo.Object, handler);

        mockClientRepo.Setup(x => x.AddAsync(It.IsAny<Client>())).ThrowsAsync(expectedEx);
        var ex = await Assert.ThrowsAsync<DataBaseException>(async () => { await clientService.Reg(login, password, mail); });

        mockClientRepo.Verify(x => x.AddAsync(It.IsAny<Client>()), Times.Once);
        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }

    [Fact]
    public async void ClientNotFoundUnRegTest()
    {
        string login = "abcd";
        string password = "1234";
        string mail = "abc@mail.ru";

        string exMessege = $"Пользователь '{login}' с почтой '{mail}' не существует";
        ExceptionType exType = ExceptionType.Error;
        ExceptionReason exReason = ExceptionReason.NotFound;

        DataBaseException expectedEx = new(exMessege, exType, exReason);

        Mock<IClientRepository> mockClientRepo = new();
        SuperHandlerException handler = new PassThroughHandlerException();
        ClientService clientService = new(mockClientRepo.Object, handler);

        mockClientRepo.Setup(x => x.GetIdByCredentialsAsync(login, password, mail)).ThrowsAsync(expectedEx);
        var ex = await Assert.ThrowsAsync<DataBaseException>(async () => { await clientService.UnReg(login, password, mail); });

        mockClientRepo.Verify(x => x.GetIdByCredentialsAsync(login, password, mail), Times.Once);
        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }

    [Fact]
    public async void ClientNotFoundSingInTest()
    {
        string login = "abcd";
        string password = "1234";
        string mail = "abc@mail.ru";

        string exMessege = $"Пользователь '{login}' с почтой '{mail}' не существует";
        ExceptionType exType = ExceptionType.Error;
        ExceptionReason exReason = ExceptionReason.NotFound;

        DataBaseException expectedEx = new(exMessege, exType, exReason);

        Mock<IClientRepository> mockClientRepo = new();
        SuperHandlerException handler = new PassThroughHandlerException();
        ClientService clientService = new(mockClientRepo.Object, handler);

        mockClientRepo.Setup(x => x.GetByCredentialsAsync(login, password, mail)).ThrowsAsync(expectedEx);
        var ex = await Assert.ThrowsAsync<DataBaseException>(async () => { await clientService.SingIn(login, password, mail); });

        mockClientRepo.Verify(x => x.GetByCredentialsAsync(login, password, mail), Times.Once);
        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }

    [Fact]
    public async void ClientNotFoundSingOutTest()
    {
        string login = "abcd";
        string password = "1234";
        string mail = "abc@mail.ru";

        string exMessege = $"Пользователь '{login}' с почтой '{mail}' не существует";
        ExceptionType exType = ExceptionType.Error;
        ExceptionReason exReason = ExceptionReason.NotFound;

        DataBaseException expectedEx = new(exMessege, exType, exReason);

        Mock<IClientRepository> mockClientRepo = new();
        SuperHandlerException handler = new PassThroughHandlerException();
        ClientService clientService = new(mockClientRepo.Object, handler);

        mockClientRepo.Setup(x => x.GetByCredentialsAsync(login, password, mail)).ThrowsAsync(expectedEx);
        var ex = await Assert.ThrowsAsync<DataBaseException>(async () => { await clientService.SingOut(login, password, mail); });

        mockClientRepo.Verify(x => x.GetByCredentialsAsync(login, password, mail), Times.Once);
        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }

    [Fact]
    public async void ClientNotFoundGetRoleTest()
    {
        string login = "abcd";
        string password = "1234";
        string mail = "abc@mail.ru";

        string exMessege = $"Пользователь '{login}' с почтой '{mail}' не существует";
        ExceptionType exType = ExceptionType.Error;
        ExceptionReason exReason = ExceptionReason.NotFound;

        DataBaseException expectedEx = new(exMessege, exType, exReason);

        Mock<IClientRepository> mockClientRepo = new();
        SuperHandlerException handler = new PassThroughHandlerException();
        ClientService clientService = new(mockClientRepo.Object, handler);

        mockClientRepo.Setup(x => x.GetByCredentialsAsync(login, password, mail)).ThrowsAsync(expectedEx);
        var ex = await Assert.ThrowsAsync<DataBaseException>(async () => { await clientService.GetRole(login, password, mail); });

        mockClientRepo.Verify(x => x.GetByCredentialsAsync(login, password, mail), Times.Once);
        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }
}