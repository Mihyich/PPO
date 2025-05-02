using MetroGid.Controllers.DTO;
using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
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
    public async void UsualSearchingAdanaTest()
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

    [Fact]
    public async void InaccessibleBranchSearchingAdanaTest()
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
        string filePath = Path.Combine(currentDirectory, "Cities", "Adana", "InaccessibleBranch.json");

        mockChartRepo.Setup(x => x.GetChartJsonAsync(city, chartTitle)).ReturnsAsync(FileReader.ReadAll(filePath));
        RouteService routeService = new(mockChartRepo.Object, mockRouteRepo.Object, handler);

        ServiceRouteException ex = await Assert.ThrowsAsync<ServiceRouteException>(async () => { await routeService.SearchRoute(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime); });

        mockChartRepo.Verify(x => x.GetChartJsonAsync(city, chartTitle), Times.Once);
        Assert.Equal($"Маршрут не удалось найти в схеме '{chartTitle}' в городе '{city}' от станции '{stationSrcTitle}' ветки '{branchSrcTitle}' до станции '{stationDstTitle}' ветки '{branchDstTitle}'", ex.Message);
        Assert.Equal(ExceptionType.Quiet, ex.ExcType);
        Assert.Equal(ExceptionReason.NotFound, ex.ExcReason);
    }

    [Fact]
    public async void InaccessibleFirstStationSearchingAdanaTest()
    {
        SuperHandlerException handler = new PassThroughHandlerException();

        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        IDomainValidatorVisitor domainReferentialityValidator = new ThrowableDomainReferentialityValidator(handler);
        
        Mock<IChartRepository> mockChartRepo = new();
        Mock<IRouteRepository> mockRouteRepo = new();

        string city = "Адана";
        string chartTitle = "Схема метро";
        string branchSrcTitle = "Линия 1";
        string stationSrcTitle = "Фатих";
        string branchDstTitle = "Линия 1";
        string stationDstTitle = "Акынджилар";
        TimeOnly startTime = new(18, 0);

        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string filePath = Path.Combine(currentDirectory, "Cities", "Adana", "InaccessibleFatih.json");

        mockChartRepo.Setup(x => x.GetChartJsonAsync(city, chartTitle)).ReturnsAsync(FileReader.ReadAll(filePath));
        RouteService routeService = new(mockChartRepo.Object, mockRouteRepo.Object, handler);

        ServiceRouteException ex = await Assert.ThrowsAsync<ServiceRouteException>(async () => { await routeService.SearchRoute(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime); });

        mockChartRepo.Verify(x => x.GetChartJsonAsync(city, chartTitle), Times.Once);
        Assert.Equal($"Маршрут не удалось найти в схеме '{chartTitle}' в городе '{city}' от станции '{stationSrcTitle}' ветки '{branchSrcTitle}' до станции '{stationDstTitle}' ветки '{branchDstTitle}'", ex.Message);
        Assert.Equal(ExceptionType.Quiet, ex.ExcType);
        Assert.Equal(ExceptionReason.NotFound, ex.ExcReason);
    }

    [Fact]
    public async void InaccessibleLastStationSearchingAdanaTest()
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
        string stationDstTitle = "Фатих";
        TimeOnly startTime = new(18, 0);

        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string filePath = Path.Combine(currentDirectory, "Cities", "Adana", "InaccessibleFatih.json");

        mockChartRepo.Setup(x => x.GetChartJsonAsync(city, chartTitle)).ReturnsAsync(FileReader.ReadAll(filePath));
        RouteService routeService = new(mockChartRepo.Object, mockRouteRepo.Object, handler);

        ServiceRouteException ex = await Assert.ThrowsAsync<ServiceRouteException>(async () => { await routeService.SearchRoute(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime); });

        mockChartRepo.Verify(x => x.GetChartJsonAsync(city, chartTitle), Times.Once);
        Assert.Equal($"Маршрут не удалось найти в схеме '{chartTitle}' в городе '{city}' от станции '{stationSrcTitle}' ветки '{branchSrcTitle}' до станции '{stationDstTitle}' ветки '{branchDstTitle}'", ex.Message);
        Assert.Equal(ExceptionType.Quiet, ex.ExcType);
        Assert.Equal(ExceptionReason.NotFound, ex.ExcReason);
    }

    [Fact]
    public async void InaccessibleOnWayStationAdanaTest()
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
        string filePath = Path.Combine(currentDirectory, "Cities", "Adana", "InaccessibleFatih.json");

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
                new RouteStationItemDTO(new StationDTO("Фатих", "Линия 1", 5, AccessTypeDTO.INACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
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

    [Fact]
    public async void UsualSearchingMoscowTest()
    {
        SuperHandlerException handler = new PassThroughHandlerException();

        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        IDomainValidatorVisitor domainReferentialityValidator = new ThrowableDomainReferentialityValidator(handler);
        
        Mock<IChartRepository> mockChartRepo = new();
        Mock<IRouteRepository> mockRouteRepo = new();

        string city = "Москва";
        string chartTitle = "Московский метрополитен";
        string branchSrcTitle = "МЦД-2";
        string stationSrcTitle = "Нахабино";
        string branchDstTitle = "Замоскворецкая линия";
        string stationDstTitle = "Алма-Атинская";
        TimeOnly startTime = new(18, 0);

        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string filePath = Path.Combine(currentDirectory, "Cities", "Moscow", "chart.json");

        mockChartRepo.Setup(x => x.GetChartJsonAsync(city, chartTitle)).ReturnsAsync(FileReader.ReadAll(filePath));
        RouteService routeService = new(mockChartRepo.Object, mockRouteRepo.Object, handler);

        RouteDTO route = await routeService.SearchRoute(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime);

        RouteDTO expectedRoute = new("Новый маршрут", city, chartTitle)
        {
            Path =
            [
                new RouteStationItemDTO(new StationDTO("Нахабино", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Нахабино", "Аникеевка", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Аникеевка", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Аникеевка", "Опалиха", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Опалиха", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Опалиха", "Красногорская", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Красногорская", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Красногорская", "Павшино", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Павшино", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Павшино", "Пенягино", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Пенягино", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Пенягино", "Волоколамская", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Волоколамская", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Волоколамская", "Трикотажная", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Трикотажная", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Трикотажная", "Тушинская", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Тушинская", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Тушинская", "Щукинская", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Щукинская", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Щукинская", "Стрешнево", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Стрешнево", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                // пересадка =======================================================================================================================================
                new RouteConnectionItemDTO(new TransitionConnectionDTO(new TransitionDTO(5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(0, 14), new TimeOnly(5, 30), new TimeOnly(1, 30), "Войковская", "Замоскворецкая линия", "Стрешнево", "МЦД-2"))),
                // пересадка =======================================================================================================================================
                new RouteStationItemDTO(new StationDTO("Войковская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Войковская", "Сокол", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Сокол", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Сокол", "Аэропорт", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Аэропорт", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Аэропорт", "Динамо", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Динамо", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Динамо", "Белорусская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Белорусская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Белорусская", "Маяковская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Маяковская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Маяковская", "Тверская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Тверская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Тверская", "Театральная", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Театральная", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Театральная", "Новокузнецкая", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Новокузнецкая", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Новокузнецкая", "Павелецкая", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Павелецкая", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Павелецкая", "Автозаводская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Автозаводская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Автозаводская", "Технопарк", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Технопарк", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Технопарк", "Коломенская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Коломенская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Коломенская", "Каширская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Каширская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Каширская", "Кантемировская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Кантемировская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Кантемировская", "Царицыно", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Царицыно", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Царицыно", "Орехово", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Орехово", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Орехово", "Домодедовская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Домодедовская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Домодедовская", "Красногвардейская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Красногвардейская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Красногвардейская", "Алма-Атинская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Алма-Атинская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
            ],
            Duration = TimeSpan.FromHours(3) + TimeSpan.FromMinutes(49) + TimeSpan.FromSeconds(10)
        };

        mockChartRepo.Verify(x => x.GetChartJsonAsync(city, chartTitle), Times.Once);
        Assert.True(route.Equals(expectedRoute));
    }

    [Fact]
    public async void InaccessibleOnWayStationSearchingMoscowTest()
    {
        SuperHandlerException handler = new PassThroughHandlerException();

        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        IDomainValidatorVisitor domainReferentialityValidator = new ThrowableDomainReferentialityValidator(handler);
        
        Mock<IChartRepository> mockChartRepo = new();
        Mock<IRouteRepository> mockRouteRepo = new();

        string city = "Москва";
        string chartTitle = "Московский метрополитен";
        string branchSrcTitle = "МЦД-2";
        string stationSrcTitle = "Нахабино";
        string branchDstTitle = "Замоскворецкая линия";
        string stationDstTitle = "Алма-Атинская";
        TimeOnly startTime = new(18, 0);

        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string filePath = Path.Combine(currentDirectory, "Cities", "Moscow", "InaccessibleStation.json");

        mockChartRepo.Setup(x => x.GetChartJsonAsync(city, chartTitle)).ReturnsAsync(FileReader.ReadAll(filePath));
        RouteService routeService = new(mockChartRepo.Object, mockRouteRepo.Object, handler);

        RouteDTO route = await routeService.SearchRoute(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime);

        RouteDTO expectedRoute = new("Новый маршрут", city, chartTitle)
        {
            Path =
            [
                new RouteStationItemDTO(new StationDTO("Нахабино", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Нахабино", "Аникеевка", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Аникеевка", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Аникеевка", "Опалиха", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Опалиха", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Опалиха", "Красногорская", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Красногорская", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Красногорская", "Павшино", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Павшино", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Павшино", "Пенягино", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Пенягино", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Пенягино", "Волоколамская", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Волоколамская", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Волоколамская", "Трикотажная", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Трикотажная", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Трикотажная", "Тушинская", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Тушинская", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Тушинская", "Щукинская", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Щукинская", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Щукинская", "Стрешнево", new TimeOnly(0, 9)))),
                new RouteStationItemDTO(new StationDTO("Стрешнево", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                // пересадка =======================================================================================================================================
                new RouteConnectionItemDTO(new TransitionConnectionDTO(new TransitionDTO(5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(0, 14), new TimeOnly(5, 30), new TimeOnly(1, 30), "Войковская", "Замоскворецкая линия", "Стрешнево", "МЦД-2"))),
                // пересадка =======================================================================================================================================
                new RouteStationItemDTO(new StationDTO("Войковская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Войковская", "Сокол", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Сокол", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Сокол", "Аэропорт", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Аэропорт", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Аэропорт", "Динамо", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Динамо", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Динамо", "Белорусская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Белорусская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Белорусская", "Маяковская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Маяковская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Маяковская", "Тверская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Тверская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Тверская", "Театральная", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Театральная", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Театральная", "Новокузнецкая", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Новокузнецкая", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Новокузнецкая", "Павелецкая", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Павелецкая", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Павелецкая", "Автозаводская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Автозаводская", "Замоскворецкая линия", 5, AccessTypeDTO.INACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Автозаводская", "Технопарк", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Технопарк", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Технопарк", "Коломенская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Коломенская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Коломенская", "Каширская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Каширская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Каширская", "Кантемировская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Кантемировская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Кантемировская", "Царицыно", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Царицыно", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Царицыно", "Орехово", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Орехово", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Орехово", "Домодедовская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Домодедовская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Домодедовская", "Красногвардейская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Красногвардейская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Красногвардейская", "Алма-Атинская", new TimeOnly(0, 5)))),
                new RouteStationItemDTO(new StationDTO("Алма-Атинская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
            ],
            Duration = TimeSpan.FromHours(3) + TimeSpan.FromMinutes(49) + TimeSpan.FromSeconds(10)
        };

        mockChartRepo.Verify(x => x.GetChartJsonAsync(city, chartTitle), Times.Once);
        Assert.True(route.Equals(expectedRoute));
    }

    [Fact]
    public async void InaccessibleFirstStationSearchingMoscowTest()
    {
        SuperHandlerException handler = new PassThroughHandlerException();

        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        IDomainValidatorVisitor domainReferentialityValidator = new ThrowableDomainReferentialityValidator(handler);
        
        Mock<IChartRepository> mockChartRepo = new();
        Mock<IRouteRepository> mockRouteRepo = new();

        string city = "Москва";
        string chartTitle = "Московский метрополитен";
        string branchSrcTitle = "Замоскворецкая линия";
        string stationSrcTitle = "Автозаводская";
        string branchDstTitle = "Замоскворецкая линия";
        string stationDstTitle = "Алма-Атинская";
        TimeOnly startTime = new(18, 0);

        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string filePath = Path.Combine(currentDirectory, "Cities", "Moscow", "InaccessibleStation.json");

        mockChartRepo.Setup(x => x.GetChartJsonAsync(city, chartTitle)).ReturnsAsync(FileReader.ReadAll(filePath));
        RouteService routeService = new(mockChartRepo.Object, mockRouteRepo.Object, handler);

        ServiceRouteException ex = await Assert.ThrowsAsync<ServiceRouteException>(async () => { await routeService.SearchRoute(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime); });

        mockChartRepo.Verify(x => x.GetChartJsonAsync(city, chartTitle), Times.Once);
        Assert.Equal($"Маршрут не удалось найти в схеме '{chartTitle}' в городе '{city}' от станции '{stationSrcTitle}' ветки '{branchSrcTitle}' до станции '{stationDstTitle}' ветки '{branchDstTitle}'", ex.Message);
        Assert.Equal(ExceptionType.Quiet, ex.ExcType);
        Assert.Equal(ExceptionReason.NotFound, ex.ExcReason);
    }

    [Fact]
    public async void InaccessibleLastStationSearchingMoscowTest()
    {
        SuperHandlerException handler = new PassThroughHandlerException();

        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        IDomainValidatorVisitor domainReferentialityValidator = new ThrowableDomainReferentialityValidator(handler);
        
        Mock<IChartRepository> mockChartRepo = new();
        Mock<IRouteRepository> mockRouteRepo = new();

        string city = "Москва";
        string chartTitle = "Московский метрополитен";
        string branchSrcTitle = "МЦД-2";
        string stationSrcTitle = "Нахабино";
        string branchDstTitle = "Замоскворецкая линия";
        string stationDstTitle = "Автозаводская";
        TimeOnly startTime = new(18, 0);

        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string filePath = Path.Combine(currentDirectory, "Cities", "Moscow", "InaccessibleStation.json");

        mockChartRepo.Setup(x => x.GetChartJsonAsync(city, chartTitle)).ReturnsAsync(FileReader.ReadAll(filePath));
        RouteService routeService = new(mockChartRepo.Object, mockRouteRepo.Object, handler);

        ServiceRouteException ex = await Assert.ThrowsAsync<ServiceRouteException>(async () => { await routeService.SearchRoute(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime); });

        mockChartRepo.Verify(x => x.GetChartJsonAsync(city, chartTitle), Times.Once);
        Assert.Equal($"Маршрут не удалось найти в схеме '{chartTitle}' в городе '{city}' от станции '{stationSrcTitle}' ветки '{branchSrcTitle}' до станции '{stationDstTitle}' ветки '{branchDstTitle}'", ex.Message);
        Assert.Equal(ExceptionType.Quiet, ex.ExcType);
        Assert.Equal(ExceptionReason.NotFound, ex.ExcReason);
    }
}