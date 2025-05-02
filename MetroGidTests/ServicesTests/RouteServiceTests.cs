using MetroGid.Core.Services;
using MetroGid.Core.Utilities.Validators.Interfaces;
using MetroGid.DBA.Interfaces;
using Moq;

namespace MetroGidTests.ServicesTests;

public class RouteServiceTests
{
    private readonly Mock<IChartRepository> mockChartRepo;
    private readonly Mock<IDomainValidatorVisitor> mockAttribsValidator;
    private readonly Mock<IDomainValidatorVisitor> mockRefValidator;
    private readonly Mock<IDomainDtoConverter> mockConverter;
    private readonly RouteService _service;

    [Fact]
    public void UsualSearchingTest()
    {
        Mock<IChartRepository> mockChartRepo = new();
        Mock<IRouteRepository> mockRouteRepo = new();

        mockChartRepo.Setup()


        RouteService routeService = new(mockChartRepo.Object, mockRouteRepo.Object);


        routeService.SearchRoute(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime);
    }
}