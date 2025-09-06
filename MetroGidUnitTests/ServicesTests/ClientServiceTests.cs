using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Interfaces;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Services;
using Moq;
using MetroGid.Core.Utility.Validators.Handlers;

namespace MetroGidUnitTests.ServicesTests;

public class ClientServiceTests
{
    [Theory]
    [InlineData("", "Aa1234", "abc@mail.ru", "Логин клиента '' имеет недопустимую длину: 0 не принадлежит [1, 255]", ExceptionType.Warning, ExceptionReason.StringLenghtOutOfRange)]
    [InlineData("abcd", "123", "abc@mail.ru", "Пароль клиента '123' имеет недопустимую длину: 3 не принадлежит [6, 255]", ExceptionType.Warning, ExceptionReason.StringLenghtOutOfRange)]
    public async void ClientAttribsRegTest(string login, string password, string mail, string exMessege, ExceptionType exType, ExceptionReason exReason)
    {
        Mock<IClientRepository> mockClientRepo = new();
        SuperExceptionHandler handler = new PassThroughHandlerException();
        ThrowableDomainAttribsValidator DomainAttribsValidator = new(handler);
        ClientService clientService = new(mockClientRepo.Object, DomainAttribsValidator, handler);
        mockClientRepo.Setup(x => x.AddAsync(It.IsAny<Client>())).ReturnsAsync(1);

        var ex = await Assert.ThrowsAsync<DomainValidationException>(async () => { await clientService.RegAsync(login, password, mail); });

        mockClientRepo.Verify(x => x.AddAsync(It.IsAny<Client>()), Times.Never);
        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }

    [Fact]
    public async void ClientLoginInUseRegTest()
    {
        string login = "abcd";
        string password = "Aa1234";
        string mail = "abc@mail.ru";
        string exMessege = $"Логин '{login}' уже занят";
        ExceptionType exType = ExceptionType.Quiet;
        ExceptionReason exReason = ExceptionReason.ItemAlreadyInUse;
        DataBaseException expectedEx = new(exMessege, exType, exReason);
        Mock<IClientRepository> mockClientRepo = new();
        SuperExceptionHandler handler = new PassThroughHandlerException();
        ThrowableDomainAttribsValidator DomainAttribsValidator = new(handler);
        ClientService clientService = new(mockClientRepo.Object, DomainAttribsValidator, handler);
        mockClientRepo.Setup(x => x.AddAsync(It.IsAny<Client>())).ThrowsAsync(expectedEx);

        var ex = await Assert.ThrowsAsync<DataBaseException>(async () => { await clientService.RegAsync(login, password, mail); });

        mockClientRepo.Verify(x => x.AddAsync(It.IsAny<Client>()), Times.Once);
        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }

    [Fact]
    public async void ClientMailInUseRegTest()
    {
        string login = "abcd";
        string password = "Aa1234";
        string mail = "abc@mail.ru";
        string exMessege = $"Почта '{mail}' уже занята";
        ExceptionType exType = ExceptionType.Quiet;
        ExceptionReason exReason = ExceptionReason.ItemAlreadyInUse;
        DataBaseException expectedEx = new(exMessege, exType, exReason);
        Mock<IClientRepository> mockClientRepo = new();
        SuperExceptionHandler handler = new PassThroughHandlerException();
        ThrowableDomainAttribsValidator DomainAttribsValidator = new(handler);
        ClientService clientService = new(mockClientRepo.Object, DomainAttribsValidator, handler);
        mockClientRepo.Setup(x => x.AddAsync(It.IsAny<Client>())).ThrowsAsync(expectedEx);

        var ex = await Assert.ThrowsAsync<DataBaseException>(async () => { await clientService.RegAsync(login, password, mail); });

        mockClientRepo.Verify(x => x.AddAsync(It.IsAny<Client>()), Times.Once);
        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }

    [Fact]
    public async void ClientNotFoundSignInTest()
    {
        string login = "abcd";
        string password = "1234";
        string exMessege = $"Неверный логин '{login}' или пароль '{password}'";
        ExceptionType exType = ExceptionType.Warning;
        ExceptionReason exReason = ExceptionReason.NotFound;
        DataBaseException expectedEx = new(exMessege, exType, exReason);
        Mock<IClientRepository> mockClientRepo = new();
        SuperExceptionHandler handler = new PassThroughHandlerException();
        ThrowableDomainAttribsValidator DomainAttribsValidator = new(handler);
        ClientService clientService = new(mockClientRepo.Object, DomainAttribsValidator, handler);
        mockClientRepo.Setup(x => x.GetByCredentialsAsync(login, password)).ReturnsAsync((Client?)null);
        
        var ex = await Assert.ThrowsAsync<DataBaseException>(async () => { await clientService.LogInAsync(login, password); });

        mockClientRepo.Verify(x => x.GetByCredentialsAsync(login, password), Times.Once);
        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }
}