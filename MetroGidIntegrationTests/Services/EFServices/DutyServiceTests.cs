using System.Data;
using MetroGid.Controllers.Utility.Interfaces;
using MCMC = MetroGid.Core.Models.Concrete;
using MCMT = MetroGid.Core.Models.Types;
using MetroGid.Core.Interfaces;
using MetroGid.Core.Services;
using MetroGid.DBA.EF.Context;
using MetroGidIntegrationTests.DBA.EFFixtures;
using MetroGidIntegrationTests.Services.EFFixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MetroGid.Core.Utility;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Utility.Validators.Interfaces;
using MetroGid.Core.Utility.Builders;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Loggers;
using MetroGid.Core.Utility.Validators.Handlers;
using MetroGid.Core.Utility.Directors;
using MetroGid.Controllers.Utility.DTO.Concrete;
using MetroGid.Core.Utility.Strategies;
using MetroGid.Core.Converters;
using Newtonsoft.Json.Linq;
using FluentAssertions;
using MetroGid.DBA.EF.Converters;

namespace MetroGidIntegrationTests.DBA.Services;

[Collection("Database")]
public class DutyServiceTests : IClassFixture<EFServiceDutyFixture>, IAsyncLifetime
{
    private readonly MetroDbContext _context, _contextSeed;
    private readonly IChartRepository _chartRepository, _chartRepositorySeed;
    private readonly IClientRepository _clientRepository, _clientRepositorySeed;
    private readonly IRouteRepository _routeRepository, _routeRepositorySeed;
    private IDbContextTransaction? _transaction;

    private readonly IChartService _chartService;
    private readonly IClientService _clientService;
    private readonly IRouteService _routeService;

    private string ChartJsonAdana;
    private string ChartJsonMoscow;
    private string ChartJsonSanktPeterburg;

    private MCMC.Chart ChartAdana;
    private MCMC.Chart ChartMoscow;
    private MCMC.Chart ChartSanktPeterburg;

    private int ChartAdanaId;
    private int ChartMoscowId;
    private int ChartSanktPeterburgId;

    private int clientId1;
    private int clientId2;
    private int clientId3;
    private int clientId4;

    private int MoscowBranchArbatPokrovId;
    private int MoscowStationIzmaylovskayaId;
    private int MoscowStationPartizanskayaId;
    private int MoscowStationSemenovskayaId;
    private int MoscowStationElectroZavodskayaId;

    private int MoscowBranchMoscowskoeCentralnoeKolcoId;
    private int MoscowStationIzmaylovoId;

    private int MoscowTransitionPartizanskayaIzmaylovoId;

    public DutyServiceTests(EFServiceDutyFixture fixture)
    {
        _context = fixture.dataBaseFixture.Context;
        _chartRepository = fixture.dataBaseFixture.chartRepository;
        _clientRepository = fixture.dataBaseFixture.clientRepository;
        _routeRepository = fixture.dataBaseFixture.routeRepository;

        _contextSeed = fixture.seedDataBaseFixture?.Context ?? throw new OperationCanceledException();
        _chartRepositorySeed = fixture.seedDataBaseFixture?.chartRepository ?? throw new OperationCanceledException();
        _clientRepositorySeed = fixture.seedDataBaseFixture?.clientRepository ?? throw new OperationCanceledException();
        _routeRepositorySeed = fixture.seedDataBaseFixture?.routeRepository ?? throw new OperationCanceledException();

        _chartService = fixture.chartService;
        _clientService = fixture.clientService;
        _routeService = fixture.routeService;

        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string AdanaChartPath = Path.Combine(currentDirectory, "Cities", "Adana", "chart.json");
        string MoscowChartPath = Path.Combine(currentDirectory, "Cities", "Moscow", "chart.json");
        string SanktPeterburgChartPath = Path.Combine(currentDirectory, "Cities", "Sankt-Peterburg", "chart.json");

        ChartJsonAdana = FileReader.ReadAll(AdanaChartPath);
        ChartJsonMoscow = FileReader.ReadAll(MoscowChartPath);
        ChartJsonSanktPeterburg = FileReader.ReadAll(SanktPeterburgChartPath);

        SuperExceptionHandler handler = new WarningHandlerException();

        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        IDomainValidatorVisitor domainReferentialityValidator = new ThrowableDomainReferentialityValidator(handler);

        BuilderChartBase builder;
        DirectorChartBase director;

        builder = new BuilderChart(domainAttribsValidator, domainReferentialityValidator);
        director = new DirectorChartJson(builder, ChartJsonAdana);
        ChartAdana = director.Construct();

        builder = new BuilderChart(domainAttribsValidator, domainReferentialityValidator);
        director = new DirectorChartJson(builder, ChartJsonMoscow);
        ChartMoscow = director.Construct();

        builder = new BuilderChart(domainAttribsValidator, domainReferentialityValidator);
        director = new DirectorChartJson(builder, ChartJsonSanktPeterburg);
        ChartSanktPeterburg = director.Construct();
    }

    public async Task InitializeAsync()
    {
        if (_contextSeed.Database.GetDbConnection().State != ConnectionState.Open)
            await _contextSeed.Database.OpenConnectionAsync();

        ChartAdanaId = await _chartRepositorySeed.AddAsync(ChartJsonAdana);
        ChartMoscowId = await _chartRepositorySeed.AddAsync(ChartJsonMoscow);
        ChartSanktPeterburgId = await _chartRepositorySeed.AddAsync(ChartJsonSanktPeterburg);

        clientId1 = await _clientRepositorySeed.AddAsync(new("Jonh", "Aa1234", "John.Tompson@mail.ru", MCMT.RoleType.DUTY));
        clientId2 = await _clientRepositorySeed.AddAsync(new("Jack", "Aa1234", "John.Vorobey@gmail.com", MCMT.RoleType.DUTY));
        clientId3 = await _clientRepositorySeed.AddAsync(new("Anna", "Aa1234", "Anna.Pilson@yandex.ru", MCMT.RoleType.DUTY));
        clientId4 = await _clientRepositorySeed.AddAsync(new("Rocky", "Aa1234", "YourBro@Boys.ru", MCMT.RoleType.DUTY));

        MoscowBranchArbatPokrovId = await _chartRepositorySeed.GetBranchIdAsync("Арбатско-Покровская линия", ChartMoscowId);
        MoscowStationIzmaylovskayaId = await _chartRepositorySeed.GetStationIdAsync("Измайловская", MoscowBranchArbatPokrovId);
        MoscowStationPartizanskayaId = await _chartRepositorySeed.GetStationIdAsync("Партизанская", MoscowBranchArbatPokrovId);
        MoscowStationSemenovskayaId = await _chartRepositorySeed.GetStationIdAsync("Семёновская", MoscowBranchArbatPokrovId);
        MoscowStationElectroZavodskayaId = await _chartRepositorySeed.GetStationIdAsync("Электрозаводская", MoscowBranchArbatPokrovId);

        MoscowBranchMoscowskoeCentralnoeKolcoId = await _chartRepositorySeed.GetBranchIdAsync("Московское центральное кольцо", ChartMoscowId);
        MoscowStationIzmaylovoId = await _chartRepositorySeed.GetStationIdAsync("Измайлово", MoscowBranchMoscowskoeCentralnoeKolcoId);

        MoscowTransitionPartizanskayaIzmaylovoId = await _chartRepositorySeed.GetTransitionIdAsync(MoscowStationPartizanskayaId, MoscowStationIzmaylovoId);

        await _clientRepositorySeed.MakeDutyOfStation(clientId1, MoscowStationIzmaylovskayaId);
        await _clientRepositorySeed.MakeDuty(clientId2, MoscowStationPartizanskayaId, MoscowTransitionPartizanskayaIzmaylovoId);
        await _clientRepositorySeed.MakeDutyOfStation(clientId1, MoscowStationSemenovskayaId);

        await _contextSeed.SaveChangesAsync();

        if (_context.Database.GetDbConnection().State != ConnectionState.Open)
            await _context.Database.OpenConnectionAsync();

        _transaction = await _context.Database.BeginTransactionAsync();

        await _context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }

        await _chartRepositorySeed.DeleteChartByIdAsync(ChartAdanaId);
        await _chartRepositorySeed.DeleteChartByIdAsync(ChartMoscowId);
        await _chartRepositorySeed.DeleteChartByIdAsync(ChartSanktPeterburgId);

        await _clientRepositorySeed.DeleteAsync(clientId1);
        await _clientRepositorySeed.DeleteAsync(clientId2);
        await _clientRepositorySeed.DeleteAsync(clientId3);
        await _clientRepositorySeed.DeleteAsync(clientId4);
    }

    [Theory]
    [InlineData("Адана", "Схема метро (Тестирование)", "Линия 1", "Больница", "Линия 1", "Акынджилар", 9, 0)]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "Арбатско-Покровская линия", "Измайловская", "Арбатско-Покровская линия", "Бауманская", 8, 30)]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "МЦД-2", "Нахабино", "МЦД-3", "Ипподром", 10, 45)]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "Калужско-Рижская линия", "Свиблово", "Филёвская линия", "Фили", 13, 15)]
    [InlineData("Санкт-Петербург", "Схема метро (Тестирование)", "Невско-Василеостровская", "Беговая", "Московско-Петроградская", "Купчино", 6, 7)]
    public async Task saveRouteDutyTest(string city, string chartTitle, string branchSrcTitle, string stationSrcTitle, string branchDstTitle, string stationDstTitle, int startHour, int startMinute)
    {
        TimeOnly timeStart = new(startHour, startMinute);
        RouteDTO serviceRoute = await _routeService.SearchRouteAsync(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, timeStart) ?? throw new OperationCanceledException();
        int chartId = await _chartRepository.GetChartIdAsync(city, chartTitle);
        ClientDTO client = new("Jonh", "Aa1234", "John.Tompson@mail.ru", RoleTypeDTO.DUTY);
        int savedRouteId = await _routeService.SaveRouteAsync(client, serviceRoute, chartId);
        string? savedRouteJson = await _routeRepository.GetByIdAsync(savedRouteId);

        MCMC.Chart[] charts = [ChartAdana, ChartMoscow, ChartSanktPeterburg];
        MCMC.Chart testChart = charts.Where(c => c.City == city && c.Title == chartTitle).FirstOrDefault() ?? throw new OperationCanceledException();
        MCMC.Station src = testChart.GetStation(branchSrcTitle, stationSrcTitle) ?? throw new OperationCanceledException();
        MCMC.Station dst = testChart.GetStation(branchDstTitle, stationDstTitle) ?? throw new OperationCanceledException();
        StrategySearchRouteBase searcher = new StrategySearchRouteBFS();
        MCMC.Route route = testChart.Search(src, dst, timeStart, searcher) ?? throw new OperationCanceledException();
        RouteDTO testRoute = DomainDtoConverter.Convert(route);

        Assert.True(chartId > 0);
        Assert.True(savedRouteId > 0);
        Assert.True(testRoute.Equals(serviceRoute));
        Assert.NotNull(savedRouteJson);
        JToken.Parse(savedRouteJson).Should().BeEquivalentTo(JToken.Parse(DomainRouteJsonConverter.Convert(route)));
    }

    [Theory]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "Арбатско-Покровская линия", "Измайловская", "Изипайловская", 10, AccessTypeDTO.INACCESSIBLE, "06:30", "01:15")]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "Арбатско-Покровская линия", "Семёновская", "Серьёзная", 1, AccessTypeDTO.INACCESSIBLE, "06:30", "01:15")]
    public async Task updateStationDutyTest(string chartCity, string chartTitle, string branchTitle, string stationTitle, string newTitle, int newOccupancy, AccessTypeDTO newType, string newOpenTime, string newCloseTime)
    {
        TimeOnly _newOpenTime = TimeConverter.FromString(newOpenTime);
        TimeOnly _newCloseTime = TimeConverter.FromString(newCloseTime);
        StationDTO newStationDTO = new(newTitle, branchTitle, newOccupancy, newType, _newOpenTime, _newCloseTime);
        int changes = await _chartService.UpdateStationAsync(RoleTypeDTO.DUTY, chartCity, chartTitle, branchTitle, stationTitle, newStationDTO);
        StationDTO? updStationDTO = await _chartService.GetStationAsync(chartCity, chartTitle, branchTitle, newStationDTO.Title);

        Assert.Equal(1, changes);
        Assert.NotNull(updStationDTO);
        Assert.Equal(newStationDTO.Title, updStationDTO.Title);
        Assert.Equal(newStationDTO.Occupancy, updStationDTO.Occupancy);
        Assert.Equal(newStationDTO.Type, updStationDTO.Type);
        Assert.Equal(newStationDTO.OpenTime, updStationDTO.OpenTime);
        Assert.Equal(newStationDTO.CloseTime, updStationDTO.CloseTime);
    }

    [Theory]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "Арбатско-Покровская линия", "Партизанская", "Московское центральное кольцо", "Измайлово", 10, AccessTypeDTO.INACCESSIBLE, "05:30", "06:30", "01:15")]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "Арбатско-Покровская линия", "Партизанская", "Московское центральное кольцо", "Измайлово", 1, AccessTypeDTO.INACCESSIBLE, "04:30", "07:30", "02:15")]
    public async Task updateTransitionDutyTest(
        string chartCity, string chartTitle, string fromBranchTitle, string fromStationTitle, string toBranchTitle, string toStationTitle,
        int newOccupancy, AccessTypeDTO newType, string newDuration, string newOpenTime, string newCloseTime)
    {
        TimeOnly _newDuration = TimeConverter.FromString(newDuration);
        TimeOnly _newOpenTime = TimeConverter.FromString(newOpenTime);
        TimeOnly _newCloseTime = TimeConverter.FromString(newCloseTime);
        TransitionDTO newTransitionDTO = new(newOccupancy, newType, _newDuration, _newOpenTime, _newCloseTime, fromStationTitle, fromBranchTitle, toStationTitle, toBranchTitle);
        int changes = await _chartService.UpdateTransitionAsync(RoleTypeDTO.DUTY, chartCity, chartTitle, fromBranchTitle, fromStationTitle, toBranchTitle, toStationTitle, newTransitionDTO);
        TransitionDTO? updTransitionDTO = await _chartService.GetTransitionAsync(chartCity, chartTitle, fromBranchTitle, fromStationTitle, toBranchTitle, toStationTitle);

        Assert.Equal(1, changes);
        Assert.NotNull(updTransitionDTO);
        Assert.Equal(newTransitionDTO.CloseTime, updTransitionDTO.CloseTime);
        Assert.Equal(newTransitionDTO.OpenTime, updTransitionDTO.OpenTime);
        Assert.Equal(newTransitionDTO.Duration, updTransitionDTO.Duration);
        Assert.Equal(newTransitionDTO.Type, updTransitionDTO.Type);
        Assert.Equal(newTransitionDTO.Occupancy, updTransitionDTO.Occupancy);
    }

    [Theory]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "Арбатско-Покровская линия", "Измайловская", "Партизанская", "01:30")]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "Арбатско-Покровская линия", "Партизанская", "Семёновская", "05:30")]
    public async Task updateRailwayDutyTest(string chartCity, string chartTitle, string branchTitle, string fromStationTitle, string toStationTitle, string newDuration)
    {
        TimeOnly _newDuration = TimeConverter.FromString(newDuration);
        RailwayDTO newRailwayDTO = new(branchTitle, fromStationTitle, toStationTitle, _newDuration);
        int changes = await _chartService.UpdateRailwayAsync(RoleTypeDTO.DUTY, chartCity, chartTitle, branchTitle, fromStationTitle, toStationTitle, newRailwayDTO);
        RailwayDTO? updRailwayDTO = await _chartService.GetRailwayAsync(chartCity, chartTitle, branchTitle, fromStationTitle, toStationTitle);

        Assert.Equal(1, changes);
        Assert.NotNull(updRailwayDTO);
        Assert.Equal(newRailwayDTO.Duration, updRailwayDTO.Duration);
    }
}