using System.Data;
using MetroGid.Controllers.Utility.Interfaces;
using MetroGid.Core.Interfaces;
using MetroGid.DBA.EF.Context;
using MetroGidIntegrationTests.Services.EFFixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MCMC = MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utility;
using MetroGid.Controllers.Utility.DTO;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Utility.Validators.Interfaces;
using MetroGid.Core.Utility.Builders;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Utility.Validators.Handlers;
using MetroGid.Core.Utility.Directors;
using MetroGid.Core.Utility.Strategies;
using MetroGid.Core.Converters;

namespace MetroGidIntegrationTests.DBA.Services;

[Collection("Database")]
public class UnsignedClientServiceTests : IClassFixture<EFServiceUnsignedFixture>, IAsyncLifetime
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

    public UnsignedClientServiceTests(EFServiceUnsignedFixture fixture)
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
    }

    [Theory]
    [InlineData("Адана", "Схема метро (Тестирование)", "Линия 1", "Больница", "Линия 1", "Акынджилар", 9, 0)]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "Арбатско-Покровская линия", "Измайловская", "Арбатско-Покровская линия", "Бауманская", 8, 30)]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "МЦД-2", "Нахабино", "МЦД-3", "Ипподром", 10, 45)]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "Калужско-Рижская линия", "Свиблово", "Филёвская линия", "Фили", 13, 15)]
    [InlineData("Санкт-Петербург", "Схема метро (Тестирование)", "Невско-Василеостровская", "Беговая", "Московско-Петроградская", "Купчино", 6, 7)]
    public async Task searchRouteTest(string city, string chartTitle, string branchSrcTitle, string stationSrcTitle, string branchDstTitle, string stationDstTitle, int startHour, int startMinute)
    {
        TimeOnly timeStart = new(startHour, startMinute);
        RouteDTO? serviceRoute = await _routeService.SearchRouteAsync(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, timeStart);

        MCMC.Chart[] charts = [ChartAdana, ChartMoscow, ChartSanktPeterburg];
        MCMC.Chart testChart = charts.Where(c => c.City == city && c.Title == chartTitle).FirstOrDefault() ?? throw new OperationCanceledException();
        MCMC.Station src = testChart.GetStation(branchSrcTitle, stationSrcTitle) ?? throw new OperationCanceledException();
        MCMC.Station dst = testChart.GetStation(branchDstTitle, stationDstTitle) ?? throw new OperationCanceledException();
        StrategySearchRouteBase searcher = new StrategySearchRouteBFS();
        RouteDTO? testRoute = DomainDtoConverter.Convert(testChart.Search(src, dst, timeStart, searcher) ?? throw new OperationCanceledException());

        Assert.NotNull(serviceRoute);
        Assert.NotNull(testRoute);
        Assert.True(testRoute?.Equals(serviceRoute) ?? false);
    }

    [Theory]
    [InlineData("Адана", "Схема метро (Тестирование)", "Линия 1", "Больница", "Линия 1", "Акынджилар", 9, 0)]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "Арбатско-Покровская линия", "Измайловская", "Арбатско-Покровская линия", "Бауманская", 8, 30)]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "МЦД-2", "Нахабино", "МЦД-3", "Ипподром", 10, 45)]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "Калужско-Рижская линия", "Свиблово", "Филёвская линия", "Фили", 13, 15)]
    [InlineData("Санкт-Петербург", "Схема метро (Тестирование)", "Невско-Василеостровская", "Беговая", "Московско-Петроградская", "Купчино", 6, 7)]
    public async Task saveRouteUnsignedTest(string city, string chartTitle, string branchSrcTitle, string stationSrcTitle, string branchDstTitle, string stationDstTitle, int startHour, int startMinute)
    {
        TimeOnly timeStart = new(startHour, startMinute);
        RouteDTO serviceRoute = await _routeService.SearchRouteAsync(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, timeStart) ?? throw new OperationCanceledException();
        int chartId = await _chartRepository.GetChartIdAsync(city, chartTitle);
        ClientDTO client = new("John", "Aa1234", "John.Tompson@mail.ru", RoleTypeDTO.UNSIGNED);
        int savedRouteId = await _routeService.SaveRouteAsync(client, serviceRoute, chartId);

        MCMC.Chart[] charts = [ChartAdana, ChartMoscow, ChartSanktPeterburg];
        MCMC.Chart testChart = charts.Where(c => c.City == city && c.Title == chartTitle).FirstOrDefault() ?? throw new OperationCanceledException();
        MCMC.Station src = testChart.GetStation(branchSrcTitle, stationSrcTitle) ?? throw new OperationCanceledException();
        MCMC.Station dst = testChart.GetStation(branchDstTitle, stationDstTitle) ?? throw new OperationCanceledException();
        StrategySearchRouteBase searcher = new StrategySearchRouteBFS();
        RouteDTO testRoute = DomainDtoConverter.Convert(testChart.Search(src, dst, timeStart, searcher) ?? throw new OperationCanceledException());

        Assert.True(chartId > 0);
        Assert.Equal(0, savedRouteId);
        Assert.True(testRoute.Equals(serviceRoute));
    }
}