using MetroGid.Controllers.Interfaces;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Services;
using MetroGid.Core.Utilities.Validators.Handlers;
using MetroGidIntegrationTests.DBA.EFFixtures;

namespace MetroGidIntegrationTests.Services.EFFixtures;

public abstract class EFServiceFixture : IDisposable
{
    public readonly EFDataBaseFixture dataBaseFixture;
    public readonly EFDataBaseFixture? seedDataBaseFixture;

    protected readonly SuperExceptionHandler exceptionHandler;
    protected readonly ThrowableDomainAttribsValidator domainAttribsValidator;
    protected readonly ThrowableDomainReferentialityValidator domainReferentialityValidator;

    public IChartService chartService { get; private set; }

    public IClientService clientService { get; private set; }

    public IRouteService routeService { get; private set; }

    public EFServiceFixture(SuperExceptionHandler handler, EFDataBaseFixture dataBaseFixture, EFDataBaseFixture? seedDataBaseFixture = null)
    {
        this.dataBaseFixture = dataBaseFixture;
        this.seedDataBaseFixture = seedDataBaseFixture;

        exceptionHandler = handler;
        domainAttribsValidator = new ThrowableDomainAttribsValidator(exceptionHandler);
        domainReferentialityValidator = new ThrowableDomainReferentialityValidator(exceptionHandler);

        chartService = new ChartService(dataBaseFixture.chartRepository, exceptionHandler);
        clientService = new ClientService(dataBaseFixture.clientRepository, domainAttribsValidator, exceptionHandler);
        routeService = new RouteService(dataBaseFixture.chartRepository, dataBaseFixture.clientRepository, dataBaseFixture.routeRepository, domainAttribsValidator, domainReferentialityValidator, exceptionHandler);
    }

    public void Dispose()
    {
        dataBaseFixture.Dispose();
        seedDataBaseFixture?.Dispose();
    }
}