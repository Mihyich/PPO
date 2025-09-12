using System.Data;
using MetroGid.Core.Interfaces;
using MCMC = MetroGid.Core.Models.Concrete;
using MCMA = MetroGid.Core.Models.Advanced;
using MetroGid.DBA.EF.Context;
using MetroGidIntegrationTests.DBA.EFFixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MCMT = MetroGid.Core.Models.Types;

namespace MetroGidIntegrationTests.DBA.EFClientRepositoryTests;

[Collection("Database")]
public class EFClientRepositoryTests : IClassFixture<EFDataBasePostgresFixture>, IDisposable
{
    private readonly MetroDbContext _context;
    private readonly IDbContextTransaction _transaction;
    private readonly IClientRepository _repository;

    public EFClientRepositoryTests(EFDataBasePostgresFixture fixture)
    {
        _context = fixture.Context;

        if (_context.Database.GetDbConnection().State != ConnectionState.Open)
            _context.Database.OpenConnection();

        _repository = fixture.clientRepository;

        _transaction = _context.Database.BeginTransaction();
    }

    [Theory]
    [InlineData("John", "Aa1234", "John.Tomson@mail.ru", MCMT.RoleType.SIGNED)]
    [InlineData("A", "A1234a", "A@gmail.com", MCMT.RoleType.SIGNED)]
    [InlineData("Robert Thomas", "1A234a", "RobTom@gmail.com", MCMT.RoleType.DUTY)]
    public async Task AddClientTest(string login, string password, string mail, MCMT.RoleType role)
    {
        MCMC.Client client = new(login, password, mail, role);

        MCMA.IdRow idRow = await _repository.AddAsync(client);

        MCMC.Client? savedClient = await _repository.GetByIdAsync(idRow.id);

        Assert.NotNull(savedClient);
        Assert.Equal(login, savedClient.Login);
        Assert.Equal(password, savedClient.Password);
        Assert.Equal(mail, savedClient.Mail);
        Assert.Equal(role, savedClient.Role);
    }

    [Fact]
    public async Task AddExistingLoginClientTest()
    {
        MCMC.Client existingClient = new(
            "AddExistingLoginClientTest",
            "Aa1234",
            "RobSom@gmail.com",
            MCMT.RoleType.SIGNED
        );

        MCMC.Client newClient = new(
            "AddExistingLoginClientTest",
            "1234Aa",
            "AlreadyExistingUser@EFClientRepositoryTests.AddExistingLoginClientTest.fact",
            MCMT.RoleType.DUTY);

        await _repository.AddAsync(existingClient);
        DbUpdateException exception = await Assert.ThrowsAsync<DbUpdateException>(() => _repository.AddAsync(newClient));
    }

    [Fact]
    public async Task AddExistingMailClientTest()
    {
        MCMC.Client existingClient = new(
            "AddExistingMailClientTest",
            "Aa1234",
            "AlreadyExistingUser@EFClientRepositoryTests.AddExistingMailClientTest.fact",
            MCMT.RoleType.SIGNED
        );

        MCMC.Client newClient = new(
            "John.AddExistingLoginClientTest",
            "1234Aa",
            "AlreadyExistingUser@EFClientRepositoryTests.AddExistingMailClientTest.fact",
            MCMT.RoleType.DUTY);

        await _repository.AddAsync(existingClient);
        DbUpdateException exception = await Assert.ThrowsAsync<DbUpdateException>(() => _repository.AddAsync(newClient));
    }

    [Fact]
    public async Task GetIdByCredentialsAsyncTest()
    {
        MCMC.Client client = new("Mohn", "Aa1234", "Mohn.Tomson@mail.ru", MCMT.RoleType.SIGNED);
        MCMA.IdRow savedIdRow = await _repository.AddAsync(client);
        MCMA.IdRow testIdRow = await _repository.GetIdByCredentialsAsync(client.Login, client.Password);
        Assert.Equal(savedIdRow, testIdRow);
    }

    [Fact]
    public async Task UpdateAsyncTest()
    {
        MCMC.Client client = new("Rohn", "Aa1234", "Rohn.Tomson@mail.ru", MCMT.RoleType.SIGNED);
        MCMC.Client updClient = new("Mark", "Aa1234", "Mark.Tomson@mail.ru", MCMT.RoleType.SIGNED);

        MCMA.IdRow savedIdRow = await _repository.AddAsync(client);
        MCMA.ChangedRowCount changesRow = await _repository.UpdateAsync(savedIdRow.id, updClient);
        MCMA.IdRow testIdRow = await _repository.GetIdByCredentialsAsync(updClient.Login, updClient.Password);
        MCMC.Client? testClient = await _repository.GetByIdAsync(savedIdRow.id);

        Assert.NotNull(testClient);
        Assert.Equal(1, changesRow.count);
        Assert.Equal(savedIdRow, testIdRow);
        Assert.Equal(updClient.Login, testClient.Login);
        Assert.Equal(updClient.Password, testClient.Password);
        Assert.Equal(updClient.Mail, testClient.Mail);
        Assert.Equal(updClient.Role, testClient.Role);
    }

    public void Dispose()
    {
        _transaction?.Rollback();
        _transaction?.Dispose();
    }
}