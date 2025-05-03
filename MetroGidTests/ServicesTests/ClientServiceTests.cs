using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Services;
using MetroGid.DBA.Interfaces;
using Moq;

namespace MetroGidTests.ServicesTests;

public class ClientServiceTests
{
    [Theory]
    [InlineData("abc", "1234", "abc@mail.ru", "Логин клиента с почтой 'abc@mail.ru' имеет недопустимую длину: 3 не принадлежит [4, 255]", ExceptionType.Warning, ExceptionReason.StringLenghtOutOfRange)]
    [InlineData("abcd", "123", "abc@mail.ru", "Пароль клиента 'abcd' с почтой 'abc@mail.ru' имеет недопустимую длину: 3 не принадлежит [4, 255]", ExceptionType.Warning, ExceptionReason.StringLenghtOutOfRange)]
    public async void ClientAttribsRegTest(string login, string password, string mail, string exMessege, ExceptionType exType, ExceptionReason exReason)
    {
        Mock<IClientRepository> mockChartRepo = new();
        SuperHandlerException handler = new PassThroughHandlerException();
        ClientService clientService = new(mockChartRepo.Object, handler);

        mockChartRepo.Setup(x => x.AddAsync(It.IsAny<Client>())).ReturnsAsync(1);
        var ex = await Assert.ThrowsAsync<DomainValidationException>(async () => { await clientService.Reg(login, password, mail); });

        mockChartRepo.Verify(x => x.AddAsync(It.IsAny<Client>()), Times.Never);
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

        ServiceRouteException expectedEx = new(exMessege, exType, exReason);

        Mock<IClientRepository> mockChartRepo = new();
        SuperHandlerException handler = new PassThroughHandlerException();
        ClientService clientService = new(mockChartRepo.Object, handler);

        mockChartRepo.Setup(x => x.AddAsync(It.IsAny<Client>())).ThrowsAsync(expectedEx);
        var ex = await Assert.ThrowsAsync<ServiceRouteException>(async () => { await clientService.Reg(login, password, mail); });

        mockChartRepo.Verify(x => x.AddAsync(It.IsAny<Client>()), Times.Once);
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

        ServiceRouteException expectedEx = new(exMessege, exType, exReason);

        Mock<IClientRepository> mockChartRepo = new();
        SuperHandlerException handler = new PassThroughHandlerException();
        ClientService clientService = new(mockChartRepo.Object, handler);

        mockChartRepo.Setup(x => x.AddAsync(It.IsAny<Client>())).ThrowsAsync(expectedEx);
        var ex = await Assert.ThrowsAsync<ServiceRouteException>(async () => { await clientService.Reg(login, password, mail); });

        mockChartRepo.Verify(x => x.AddAsync(It.IsAny<Client>()), Times.Once);
        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }
}