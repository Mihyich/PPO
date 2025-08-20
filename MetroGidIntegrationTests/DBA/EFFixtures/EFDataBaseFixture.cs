using MetroGid.Core.Interfaces;
using MetroGid.DBA.EF.Context;
using MetroGid.DBA.EF.Repositories;
using MetroGid.Environment.Concrete;
using MetroGid.Environment.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MetroGidIntegrationTests.DBA.EFFixtures;

public class EFDataBaseFixture : IDisposable
{
    public MetroDbContext Context { get; private set; }

    public IClientRepository clientRepository { get; private set; }

    public IRouteRepository routeRepository { get; private set; }

    public EFDataBaseFixture()
    {
        IEnvironmentLoader environmentLoader = new DevelopmentEnvironmentLoader();
        environmentLoader.Load();

        IConfigurationRoot config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        string connectionString = config.GetConnectionString("MetroDb") ??
            throw new InvalidOperationException("Не найдена конфигурация подключения");

        DbContextOptions<MetroDbContext> dbContextOptions = new DbContextOptionsBuilder<MetroDbContext>()
            .UseNpgsql(connectionString, o => o.SetPostgresVersion(16, 9))
            .Options;

        Context = new MetroDbContext(dbContextOptions);
        clientRepository = new EFClientRepository(Context);
        routeRepository = new EFRouteRepository(Context);
    }

    public void Dispose()
    {
        Context?.Dispose();
    }
}