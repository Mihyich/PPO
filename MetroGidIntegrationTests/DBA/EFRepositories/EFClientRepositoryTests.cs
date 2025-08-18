using System.Data;
using MetroGid.Core.Interfaces;
using MCMC = MetroGid.Core.Models.Concrete;
using MetroGid.DBA.EF.Context;
using MetroGidIntegrationTests.DBA.EFFixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MCMT = MetroGid.Core.Models.Types;
using MetroGid.DBA.EF.Converters;
using MDEMT = MetroGid.DBA.EF.Models.Tables;

namespace MetroGidIntegrationTests.DBA.EFClientRepositoryTests;

public class EFClientRepositoryTests : IClassFixture<EFDataBaseFixture>, IDisposable
{
    private readonly MetroDbContext _context;
    private readonly IDbContextTransaction _transaction;
    private readonly IClientRepository _repository;

    public EFClientRepositoryTests(EFDataBaseFixture fixture)
    {
        _context = fixture.Context;

        if (_context.Database.GetDbConnection().State != ConnectionState.Open)
            _context.Database.OpenConnection();

        _repository = fixture.clientRepository;

        _transaction = _context.Database.BeginTransaction();
    }

    [Theory]
    [InlineData("John", "Aa1234", "John.Tomson@mail.ru", MCMT.RoleType.SIGNED)]
    public async Task AddClientTest(string login, string password, string mail, MCMT.RoleType role)
    {
        MCMC.Client client = new(login, password, mail, role);

        int id = await _repository.AddAsync(client);

        MCMC.Client savedClient = await _repository.GetByIdAsync(id);

        Assert.NotNull(savedClient);
        Assert.Equal(login, savedClient.Login);
        Assert.Equal(password, savedClient.Password);
        Assert.Equal(mail, savedClient.Mail);
        Assert.Equal(role, savedClient.Role);
    }

    // Этот метод вызывается после каждого теста
    public void Dispose()
    {
        _transaction?.Rollback(); // откат изменений
        _transaction?.Dispose();
    }
}