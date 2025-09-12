using MetroGid.Controllers.Utility.DTO.Concrete;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Models.Advanced;
using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Services;
using MetroGid.Core.Utility;
using MetroGid.Core.Interfaces;
using MetroGid.Core.Utility.Validators.Handlers;
using Moq;
using MetroGid.Core.Converters;

namespace MetroGidUnitTests.ServicesTests;

public class RouteServiceTests
{
    [Fact]
    public async void UsualSearchingAdanaTest()
    {
        SuperExceptionHandler handler = new PassThroughHandlerException();
        ThrowableDomainAttribsValidator domainAttribsValidator = new(handler);
        ThrowableDomainReferentialityValidator domainReferentialityValidator = new(handler);
        
        Mock<IChartRepository> mockChartRepo = new();
        Mock<IClientRepository> mockClientRepo = new();
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

        mockChartRepo.Setup(x => x.GetChartJsonByCredentialsAsync(city, chartTitle)).ReturnsAsync(new FileRow(FileReader.ReadAll(filePath)));
        RouteService routeService = new(mockChartRepo.Object, mockClientRepo.Object, mockRouteRepo.Object, domainAttribsValidator, domainReferentialityValidator, handler);

        RouteDTO expectedRoute = new(
            $"[\"{city}\":\"{chartTitle}\"]:[\"{branchSrcTitle}\":\"{stationSrcTitle}\"]:[\"{branchDstTitle}\":\"{stationDstTitle}\"]",
            city,
            chartTitle,
            [
                new RouteStationItemDTO(new StationDTO("Больница", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Больница", "Анатолийский лицей", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Анатолийский лицей", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Анатолийский лицей", "Хузуреви", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Хузуреви", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Хузуреви", "Бульвар Мави", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Бульвар Мави", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Бульвар Мави", "Юрт", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Юрт", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Юрт", "Ешильюрт", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Ешильюрт", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Ешильюрт", "Фатих", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Фатих", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Фатих", "Вилайет", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Вилайет", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Вилайет", "Истикляль", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Истикляль", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Истикляль", "Коджавезир", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Коджавезир", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Коджавезир", "Хюрриет", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Хюрриет", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Хюрриет", "Джумхуриет", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Джумхуриет", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Джумхуриет", "Акынджилар", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Акынджилар", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30)))
            ],
            TimeSpan.FromMinutes(2 * 60 + 6)
        );

        Route? route = await routeService.SearchRouteAsync(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime);

        mockChartRepo.Verify(x => x.GetChartJsonByCredentialsAsync(city, chartTitle), Times.Once);
        Assert.NotNull(route);
        Assert.True(DomainDtoConverter.Convert(route)?.Equals(expectedRoute) ?? false);
    }

    [Fact]
    public async void InaccessibleBranchSearchingAdanaTest()
    {
        SuperExceptionHandler handler = new PassThroughHandlerException();
        ThrowableDomainAttribsValidator domainAttribsValidator = new(handler);
        ThrowableDomainReferentialityValidator domainReferentialityValidator = new(handler);

        Mock<IChartRepository> mockChartRepo = new();
        Mock<IClientRepository> mockClientRepo = new();
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

        mockChartRepo.Setup(x => x.GetChartJsonByCredentialsAsync(city, chartTitle)).ReturnsAsync(new FileRow(FileReader.ReadAll(filePath)));
        RouteService routeService = new(mockChartRepo.Object, mockClientRepo.Object, mockRouteRepo.Object, domainAttribsValidator, domainReferentialityValidator, handler);

        Route? route = await routeService.SearchRouteAsync(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime);

        mockChartRepo.Verify(x => x.GetChartJsonByCredentialsAsync(city, chartTitle), Times.Once);
        Assert.Null(route);
    }

    [Fact]
    public async void InaccessibleFirstStationSearchingAdanaTest()
    {
        SuperExceptionHandler handler = new PassThroughHandlerException();
        ThrowableDomainAttribsValidator domainAttribsValidator = new(handler);
        ThrowableDomainReferentialityValidator domainReferentialityValidator = new(handler);
        
        Mock<IChartRepository> mockChartRepo = new();
        Mock<IClientRepository> mockClientRepo = new();
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

        mockChartRepo.Setup(x => x.GetChartJsonByCredentialsAsync(city, chartTitle)).ReturnsAsync(new FileRow(FileReader.ReadAll(filePath)));
        RouteService routeService = new(mockChartRepo.Object, mockClientRepo.Object, mockRouteRepo.Object, domainAttribsValidator, domainReferentialityValidator, handler);

        Route? route = await routeService.SearchRouteAsync(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime);

        mockChartRepo.Verify(x => x.GetChartJsonByCredentialsAsync(city, chartTitle), Times.Once);
        Assert.Null(route);
    }

    [Fact]
    public async void InaccessibleLastStationSearchingAdanaTest()
    {
        SuperExceptionHandler handler = new PassThroughHandlerException();
        ThrowableDomainAttribsValidator domainAttribsValidator = new(handler);
        ThrowableDomainReferentialityValidator domainReferentialityValidator = new(handler);
        
        Mock<IChartRepository> mockChartRepo = new();
        Mock<IClientRepository> mockClientRepo = new();
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

        mockChartRepo.Setup(x => x.GetChartJsonByCredentialsAsync(city, chartTitle)).ReturnsAsync(new FileRow(FileReader.ReadAll(filePath)));
        RouteService routeService = new(mockChartRepo.Object, mockClientRepo.Object, mockRouteRepo.Object, domainAttribsValidator, domainReferentialityValidator, handler);

        Route? route = await routeService.SearchRouteAsync(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime);

        mockChartRepo.Verify(x => x.GetChartJsonByCredentialsAsync(city, chartTitle), Times.Once);
        Assert.Null(route);
    }

    [Fact]
    public async void InaccessibleOnWayStationAdanaTest()
    {
        SuperExceptionHandler handler = new PassThroughHandlerException();
        ThrowableDomainAttribsValidator domainAttribsValidator = new(handler);
        ThrowableDomainReferentialityValidator domainReferentialityValidator = new(handler);
        
        Mock<IChartRepository> mockChartRepo = new();
        Mock<IClientRepository> mockClientRepo = new();
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

        mockChartRepo.Setup(x => x.GetChartJsonByCredentialsAsync(city, chartTitle)).ReturnsAsync(new FileRow(FileReader.ReadAll(filePath)));
        RouteService routeService = new(mockChartRepo.Object, mockClientRepo.Object, mockRouteRepo.Object, domainAttribsValidator, domainReferentialityValidator, handler);

        Route? route = await routeService.SearchRouteAsync(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime);

        RouteDTO expectedRoute = new(
            $"[\"{city}\":\"{chartTitle}\"]:[\"{branchSrcTitle}\":\"{stationSrcTitle}\"]:[\"{branchDstTitle}\":\"{stationDstTitle}\"]",
            city,
            chartTitle,
            [
                new RouteStationItemDTO(new StationDTO("Больница", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Больница", "Анатолийский лицей", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Анатолийский лицей", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Анатолийский лицей", "Хузуреви", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Хузуреви", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Хузуреви", "Бульвар Мави", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Бульвар Мави", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Бульвар Мави", "Юрт", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Юрт", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Юрт", "Ешильюрт", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Ешильюрт", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Ешильюрт", "Фатих", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Фатих", "Линия 1", 5, AccessTypeDTO.INACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Фатих", "Вилайет", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Вилайет", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Вилайет", "Истикляль", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Истикляль", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Истикляль", "Коджавезир", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Коджавезир", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Коджавезир", "Хюрриет", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Хюрриет", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Хюрриет", "Джумхуриет", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Джумхуриет", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Линия 1", "Джумхуриет", "Акынджилар", new TimeSpan(0, 10, 0)))),
                new RouteStationItemDTO(new StationDTO("Акынджилар", "Линия 1", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30)))
            ],
            TimeSpan.FromMinutes(2 * 60 + 6)
        );

        mockChartRepo.Verify(x => x.GetChartJsonByCredentialsAsync(city, chartTitle), Times.Once);
        Assert.NotNull(route);
        Assert.True(DomainDtoConverter.Convert(route)?.Equals(expectedRoute) ?? false);
    }

    [Fact]
    public async void NotFoundDstStationSearchingAdanaTest()
    {
        SuperExceptionHandler handler = new PassThroughHandlerException();
        ThrowableDomainAttribsValidator domainAttribsValidator = new(handler);
        ThrowableDomainReferentialityValidator domainReferentialityValidator = new(handler);
        
        Mock<IChartRepository> mockChartRepo = new();
        Mock<IClientRepository> mockClientRepo = new();
        Mock<IRouteRepository> mockRouteRepo = new();

        string city = "Адана";
        string chartTitle = "Схема метро";
        string branchSrcTitle = "МЦД-2";
        string stationSrcTitle = "Нахабино";
        string branchDstTitle = "линия";
        string stationDstTitle = "Автозаводская";
        TimeOnly startTime = new(18, 0);

        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string filePath = Path.Combine(currentDirectory, "Cities", "Adana", "chart.json");

        mockChartRepo.Setup(x => x.GetChartJsonByCredentialsAsync(city, chartTitle)).ReturnsAsync(new FileRow(FileReader.ReadAll(filePath)));
        RouteService routeService = new(mockChartRepo.Object, mockClientRepo.Object, mockRouteRepo.Object, domainAttribsValidator, domainReferentialityValidator, handler);

        ServiceRouteException ex = await Assert.ThrowsAsync<ServiceRouteException>(async () => { await routeService.SearchRouteAsync(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime); });

        mockChartRepo.Verify(x => x.GetChartJsonByCredentialsAsync(city, chartTitle), Times.Once);
        Assert.Equal($"В схеме '{chartTitle}' для города '{city}' не найдена станция '{stationSrcTitle}' ветки '{branchSrcTitle}'", ex.Message);
        Assert.Equal(ExceptionType.Error, ex.ExcType);
        Assert.Equal(ExceptionReason.NotFound, ex.ExcReason);
    }

    [Fact]
    public async void UsualSearchingMoscowTest()
    {
        SuperExceptionHandler handler = new PassThroughHandlerException();
        ThrowableDomainAttribsValidator domainAttribsValidator = new(handler);
        ThrowableDomainReferentialityValidator domainReferentialityValidator = new(handler);
        
        Mock<IChartRepository> mockChartRepo = new();
        Mock<IClientRepository> mockClientRepo = new();
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

        mockChartRepo.Setup(x => x.GetChartJsonByCredentialsAsync(city, chartTitle)).ReturnsAsync(new FileRow(FileReader.ReadAll(filePath)));
        RouteService routeService = new(mockChartRepo.Object, mockClientRepo.Object, mockRouteRepo.Object, domainAttribsValidator, domainReferentialityValidator, handler);

        Route? route = await routeService.SearchRouteAsync(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime);

        RouteDTO expectedRoute = new(
            $"[\"{city}\":\"{chartTitle}\"]:[\"{branchSrcTitle}\":\"{stationSrcTitle}\"]:[\"{branchDstTitle}\":\"{stationDstTitle}\"]",
            city,
            chartTitle,
            [
                new RouteStationItemDTO(new StationDTO("Нахабино", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Нахабино", "Аникеевка", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Аникеевка", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Аникеевка", "Опалиха", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Опалиха", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Опалиха", "Красногорская", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Красногорская", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Красногорская", "Павшино", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Павшино", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Павшино", "Пенягино", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Пенягино", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Пенягино", "Волоколамская", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Волоколамская", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Волоколамская", "Трикотажная", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Трикотажная", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Трикотажная", "Тушинская", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Тушинская", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Тушинская", "Щукинская", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Щукинская", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Щукинская", "Стрешнево", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Стрешнево", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                // пересадка =======================================================================================================================================
                new RouteConnectionItemDTO(new TransitionConnectionDTO(new TransitionDTO(5, AccessTypeDTO.ACCESSIBLE, new TimeSpan(0, 14, 0), new TimeOnly(5, 30), new TimeOnly(1, 30), "Стрешнево", "МЦД-2", "Войковская", "Замоскворецкая линия"))),
                // пересадка =======================================================================================================================================
                new RouteStationItemDTO(new StationDTO("Войковская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Войковская", "Сокол", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Сокол", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Сокол", "Аэропорт", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Аэропорт", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Аэропорт", "Динамо", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Динамо", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Динамо", "Белорусская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Белорусская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Белорусская", "Маяковская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Маяковская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Маяковская", "Тверская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Тверская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Тверская", "Театральная", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Театральная", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Театральная", "Новокузнецкая", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Новокузнецкая", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Новокузнецкая", "Павелецкая", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Павелецкая", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Павелецкая", "Автозаводская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Автозаводская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Автозаводская", "Технопарк", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Технопарк", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Технопарк", "Коломенская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Коломенская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Коломенская", "Каширская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Каширская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Каширская", "Кантемировская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Кантемировская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Кантемировская", "Царицыно", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Царицыно", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Царицыно", "Орехово", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Орехово", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Орехово", "Домодедовская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Домодедовская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Домодедовская", "Красногвардейская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Красногвардейская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Красногвардейская", "Алма-Атинская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Алма-Атинская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
            ],
            TimeSpan.FromHours(3) + TimeSpan.FromMinutes(56) + TimeSpan.FromSeconds(10)
        );

        RouteDTO? routeDTO = route != null ? DomainDtoConverter.Convert(route) : null;

        mockChartRepo.Verify(x => x.GetChartJsonByCredentialsAsync(city, chartTitle), Times.Once);
        Assert.NotNull(route);
        Assert.True(routeDTO?.Equals(expectedRoute) ?? false);
    }

    [Fact]
    public async void InaccessibleOnWayStationSearchingMoscowTest()
    {
        SuperExceptionHandler handler = new PassThroughHandlerException();
        ThrowableDomainAttribsValidator domainAttribsValidator = new(handler);
        ThrowableDomainReferentialityValidator domainReferentialityValidator = new(handler);
        
        Mock<IChartRepository> mockChartRepo = new();
        Mock<IClientRepository> mockClientRepo = new();
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

        mockChartRepo.Setup(x => x.GetChartJsonByCredentialsAsync(city, chartTitle)).ReturnsAsync(new FileRow(FileReader.ReadAll(filePath)));
        RouteService routeService = new(mockChartRepo.Object, mockClientRepo.Object, mockRouteRepo.Object, domainAttribsValidator, domainReferentialityValidator, handler);

        Route? route = await routeService.SearchRouteAsync(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime);

        RouteDTO expectedRoute = new(
            $"[\"{city}\":\"{chartTitle}\"]:[\"{branchSrcTitle}\":\"{stationSrcTitle}\"]:[\"{branchDstTitle}\":\"{stationDstTitle}\"]",
            city,
            chartTitle,
            [
                new RouteStationItemDTO(new StationDTO("Нахабино", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Нахабино", "Аникеевка", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Аникеевка", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Аникеевка", "Опалиха", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Опалиха", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Опалиха", "Красногорская", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Красногорская", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Красногорская", "Павшино", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Павшино", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Павшино", "Пенягино", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Пенягино", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Пенягино", "Волоколамская", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Волоколамская", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Волоколамская", "Трикотажная", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Трикотажная", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Трикотажная", "Тушинская", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Тушинская", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Тушинская", "Щукинская", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Щукинская", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("МЦД-2", "Щукинская", "Стрешнево", new TimeSpan(0, 9, 0)))),
                new RouteStationItemDTO(new StationDTO("Стрешнево", "МЦД-2", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                // пересадка =======================================================================================================================================
                new RouteConnectionItemDTO(new TransitionConnectionDTO(new TransitionDTO(5, AccessTypeDTO.ACCESSIBLE, new TimeSpan(0, 14, 0), new TimeOnly(5, 30), new TimeOnly(1, 30), "Стрешнево", "МЦД-2", "Войковская", "Замоскворецкая линия"))),
                // пересадка =======================================================================================================================================
                new RouteStationItemDTO(new StationDTO("Войковская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Войковская", "Сокол", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Сокол", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Сокол", "Аэропорт", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Аэропорт", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Аэропорт", "Динамо", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Динамо", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Динамо", "Белорусская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Белорусская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Белорусская", "Маяковская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Маяковская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Маяковская", "Тверская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Тверская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Тверская", "Театральная", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Театральная", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Театральная", "Новокузнецкая", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Новокузнецкая", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Новокузнецкая", "Павелецкая", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Павелецкая", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Павелецкая", "Автозаводская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Автозаводская", "Замоскворецкая линия", 5, AccessTypeDTO.INACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Автозаводская", "Технопарк", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Технопарк", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Технопарк", "Коломенская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Коломенская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Коломенская", "Каширская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Каширская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Каширская", "Кантемировская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Кантемировская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Кантемировская", "Царицыно", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Царицыно", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Царицыно", "Орехово", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Орехово", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Орехово", "Домодедовская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Домодедовская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Домодедовская", "Красногвардейская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Красногвардейская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
                new RouteConnectionItemDTO(new RailwayConnectionDTO(new RailwayDTO("Замоскворецкая линия", "Красногвардейская", "Алма-Атинская", new TimeSpan(0, 5, 0)))),
                new RouteStationItemDTO(new StationDTO("Алма-Атинская", "Замоскворецкая линия", 5, AccessTypeDTO.ACCESSIBLE, new TimeOnly(5, 30), new TimeOnly(1, 30))),
            ],
            TimeSpan.FromHours(3) + TimeSpan.FromMinutes(56) + TimeSpan.FromSeconds(10)
        );

        mockChartRepo.Verify(x => x.GetChartJsonByCredentialsAsync(city, chartTitle), Times.Once);
        Assert.NotNull(route);
        Assert.True(DomainDtoConverter.Convert(route)?.Equals(expectedRoute) ?? false);
    }

    [Fact]
    public async void InaccessibleFirstStationSearchingMoscowTest()
    {
        SuperExceptionHandler handler = new PassThroughHandlerException();
        ThrowableDomainAttribsValidator domainAttribsValidator = new(handler);
        ThrowableDomainReferentialityValidator domainReferentialityValidator = new(handler);
        
        Mock<IChartRepository> mockChartRepo = new();
        Mock<IClientRepository> mockClientRepo = new();
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

        mockChartRepo.Setup(x => x.GetChartJsonByCredentialsAsync(city, chartTitle)).ReturnsAsync(new FileRow(FileReader.ReadAll(filePath)));
        RouteService routeService = new(mockChartRepo.Object, mockClientRepo.Object, mockRouteRepo.Object, domainAttribsValidator, domainReferentialityValidator, handler);

        Route? route = await routeService.SearchRouteAsync(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime);

        mockChartRepo.Verify(x => x.GetChartJsonByCredentialsAsync(city, chartTitle), Times.Once);
        Assert.Null(route);
    }

    [Fact]
    public async void InaccessibleLastStationSearchingMoscowTest()
    {
        SuperExceptionHandler handler = new PassThroughHandlerException();
        ThrowableDomainAttribsValidator domainAttribsValidator = new(handler);
        ThrowableDomainReferentialityValidator domainReferentialityValidator = new(handler);
        
        Mock<IChartRepository> mockChartRepo = new();
        Mock<IClientRepository> mockClientRepo = new();
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

        mockChartRepo.Setup(x => x.GetChartJsonByCredentialsAsync(city, chartTitle)).ReturnsAsync(new FileRow(FileReader.ReadAll(filePath)));
        RouteService routeService = new(mockChartRepo.Object, mockClientRepo.Object, mockRouteRepo.Object, domainAttribsValidator, domainReferentialityValidator, handler);

        Route? route = await routeService.SearchRouteAsync(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime);

        mockChartRepo.Verify(x => x.GetChartJsonByCredentialsAsync(city, chartTitle), Times.Once);
        Assert.Null(route);
    }

    [Fact]
    public async void NotFoundSrcStationSearchingMoscowTest()
    {
        SuperExceptionHandler handler = new PassThroughHandlerException();
        ThrowableDomainAttribsValidator domainAttribsValidator = new(handler);
        ThrowableDomainReferentialityValidator domainReferentialityValidator = new(handler);
        
        Mock<IChartRepository> mockChartRepo = new();
        Mock<IClientRepository> mockClientRepo = new();
        Mock<IRouteRepository> mockRouteRepo = new();

        string city = "Москва";
        string chartTitle = "Московский метрополитен";
        string branchSrcTitle = "МЦД-113";
        string stationSrcTitle = "Нахабино";
        string branchDstTitle = "Замоскворецкая линия";
        string stationDstTitle = "Автозаводская";
        TimeOnly startTime = new(18, 0);

        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string filePath = Path.Combine(currentDirectory, "Cities", "Moscow", "InaccessibleStation.json");

        mockChartRepo.Setup(x => x.GetChartJsonByCredentialsAsync(city, chartTitle)).ReturnsAsync(new FileRow(FileReader.ReadAll(filePath)));
        RouteService routeService = new(mockChartRepo.Object, mockClientRepo.Object, mockRouteRepo.Object, domainAttribsValidator, domainReferentialityValidator, handler);

        ServiceRouteException ex = await Assert.ThrowsAsync<ServiceRouteException>(async () => { await routeService.SearchRouteAsync(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime); });

        mockChartRepo.Verify(x => x.GetChartJsonByCredentialsAsync(city, chartTitle), Times.Once);
        Assert.Equal($"В схеме '{chartTitle}' для города '{city}' не найдена станция '{stationSrcTitle}' ветки '{branchSrcTitle}'", ex.Message);
        Assert.Equal(ExceptionType.Error, ex.ExcType);
        Assert.Equal(ExceptionReason.NotFound, ex.ExcReason);
    }

    [Fact]
    public async void NotFoundDstStationSearchingMoscowTest()
    {
        SuperExceptionHandler handler = new PassThroughHandlerException();
        ThrowableDomainAttribsValidator domainAttribsValidator = new(handler);
        ThrowableDomainReferentialityValidator domainReferentialityValidator = new(handler);
        
        Mock<IChartRepository> mockChartRepo = new();
        Mock<IClientRepository> mockClientRepo = new();
        Mock<IRouteRepository> mockRouteRepo = new();

        string city = "Москва";
        string chartTitle = "Московский метрополитен";
        string branchSrcTitle = "МЦД-2";
        string stationSrcTitle = "Нахабино";
        string branchDstTitle = "линия";
        string stationDstTitle = "Автозаводская";
        TimeOnly startTime = new(18, 0);

        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string filePath = Path.Combine(currentDirectory, "Cities", "Moscow", "InaccessibleStation.json");

        mockChartRepo.Setup(x => x.GetChartJsonByCredentialsAsync(city, chartTitle)).ReturnsAsync(new FileRow(FileReader.ReadAll(filePath)));
        RouteService routeService = new(mockChartRepo.Object, mockClientRepo.Object, mockRouteRepo.Object, domainAttribsValidator, domainReferentialityValidator, handler);

        ServiceRouteException ex = await Assert.ThrowsAsync<ServiceRouteException>(async () => { await routeService.SearchRouteAsync(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, startTime); });

        mockChartRepo.Verify(x => x.GetChartJsonByCredentialsAsync(city, chartTitle), Times.Once);
        Assert.Equal($"В схеме '{chartTitle}' для города '{city}' не найдена станция '{stationDstTitle}' ветки '{branchDstTitle}'", ex.Message);
        Assert.Equal(ExceptionType.Error, ex.ExcType);
        Assert.Equal(ExceptionReason.NotFound, ex.ExcReason);
    }
}