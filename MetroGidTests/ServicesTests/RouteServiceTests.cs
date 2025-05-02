using MetroGid.Controllers.DTO;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Services;
using MetroGid.Core.Utilities;
using MetroGid.Core.Utilities.Validators.Handlers;
using MetroGid.Core.Utilities.Validators.Interfaces;
using MetroGid.DBA.Interfaces;
using Moq;

namespace MetroGidTests.ServicesTests;

public class RouteServiceTests
{
    [Fact]
    public async void UsualSearchingTest()
    {
        SuperHandlerException handler = new PassThroughHandlerException();

        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        IDomainValidatorVisitor domainReferentialityValidator = new ThrowableDomainReferentialityValidator(handler);
        
        Mock<IChartRepository> mockChartRepo = new();
        Mock<IRouteRepository> mockRouteRepo = new();

        string city = "Адана";
        string chartTitle = "Схема метро";
        string branchSrcTitle = "Линия 1";
        string stationSrcTitle = "Больница";
        string branchDstTitle = "Линия 1";
        string stationDstTitle = "Акынджилар";
        TimeOnly startTime = new(18, 0);

        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string filePath = Path.Combine(currentDirectory, "Cities", "Adana", "chart.json");

        mockChartRepo.Setup(x => x.GetChartJsonAsync(city, chartTitle)).ReturnsAsync(FileReader.ReadAll(filePath));
        RouteService routeService = new(mockChartRepo.Object, mockRouteRepo.Object, handler);

        RouteDTO route = await routeService.SearchRoute(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime);

        RouteDTO expectedRoute = new("Новый маршрут", city, chartTitle)
        {
            Path =
            [
                new RouteStationItemDTO(new StationDTO("Больница", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Больница", "Анатолийский лицей", new TimeOnly(0, 10)))),
                new RouteStationItemDTO(new StationDTO("Анатолийский лицей", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Анатолийский лицей", "Хузуреви", new TimeOnly(0, 10)))),
                new RouteStationItemDTO(new StationDTO("Хузуреви", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Хузуреви", "Бульвар Мави", new TimeOnly(0, 10)))),
                new RouteStationItemDTO(new StationDTO("Бульвар Мави", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Бульвар Мави", "Юрт", new TimeOnly(0, 10)))),
                new RouteStationItemDTO(new StationDTO("Юрт", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Юрт", "Ешильюрт", new TimeOnly(0, 10)))),
                new RouteStationItemDTO(new StationDTO("Ешильюрт", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Ешильюрт", "Фатих", new TimeOnly(0, 10)))),
                new RouteStationItemDTO(new StationDTO("Фатих", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Фатих", "Вилайет", new TimeOnly(0, 10)))),
                new RouteStationItemDTO(new StationDTO("Вилайет", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Вилайет", "Истикляль", new TimeOnly(0, 10)))),
                new RouteStationItemDTO(new StationDTO("Истикляль", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Истикляль", "Коджавезир", new TimeOnly(0, 10)))),
                new RouteStationItemDTO(new StationDTO("Коджавезир", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Коджавезир", "Хюрриет", new TimeOnly(0, 10)))),
                new RouteStationItemDTO(new StationDTO("Хюрриет", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Хюрриет", "Джумхуриет", new TimeOnly(0, 10)))),
                new RouteStationItemDTO(new StationDTO("Джумхуриет", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Джумхуриет", "Акынджилар", new TimeOnly(0, 10)))),
                new RouteStationItemDTO(new StationDTO("Акынджилар", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30)))
            ],
            Duration = TimeSpan.FromMinutes(2 * 60 + 6)
        };

        mockChartRepo.Verify(x => x.GetChartJsonAsync(city, chartTitle), Times.Once);
        Assert.True(route.Equals(expectedRoute));
    }
}