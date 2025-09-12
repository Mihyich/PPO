using System.Data;
using MetroGid.Controllers.Utility.Interfaces;
using MCMC = MetroGid.Core.Models.Concrete;
using MCMA = MetroGid.Core.Models.Advanced;
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

    private MCMA.IdRow? ChartAdanaIdRow;
    private MCMA.IdRow? ChartMoscowIdRow;
    private MCMA.IdRow? ChartSanktPeterburgIdRow;

    private MCMA.IdRow? clientId1Row;
    private MCMA.IdRow? clientId2Row;
    private MCMA.IdRow? clientId3Row;
    private MCMA.IdRow? clientId4Row;

    private MCMA.IdRow? MoscowBranchArbatPokrovIdRow;
    private MCMA.IdRow? MoscowStationIzmaylovskayaIdRow;
    private MCMA.IdRow? MoscowStationPartizanskayaIdRow;
    private MCMA.IdRow? MoscowStationSemenovskayaIdRow;
    private MCMA.IdRow? MoscowStationElectroZavodskayaIdRow;

    private MCMA.IdRow? MoscowBranchMoscowskoeCentralnoeKolcoIdRow;
    private MCMA.IdRow? MoscowStationIzmaylovoIdRow;

    private MCMA.IdRow? MoscowTransitionPartizanskayaIzmaylovoIdRow;

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

        ChartAdanaIdRow = await _chartRepositorySeed.AddAsync(ChartJsonAdana);
        ChartMoscowIdRow = await _chartRepositorySeed.AddAsync(ChartJsonMoscow);
        ChartSanktPeterburgIdRow = await _chartRepositorySeed.AddAsync(ChartJsonSanktPeterburg);

        clientId1Row = await _clientRepositorySeed.AddAsync(new("Jonh", "Aa1234", "John.Tompson@mail.ru", MCMT.RoleType.DUTY));
        clientId2Row = await _clientRepositorySeed.AddAsync(new("Jack", "Aa1234", "John.Vorobey@gmail.com", MCMT.RoleType.DUTY));
        clientId3Row = await _clientRepositorySeed.AddAsync(new("Anna", "Aa1234", "Anna.Pilson@yandex.ru", MCMT.RoleType.DUTY));
        clientId4Row = await _clientRepositorySeed.AddAsync(new("Rocky", "Aa1234", "YourBro@Boys.ru", MCMT.RoleType.DUTY));

        MoscowBranchArbatPokrovIdRow = await _chartRepositorySeed.GetBranchIdAsync("Арбатско-Покровская линия", ChartMoscowIdRow.id);
        MoscowStationIzmaylovskayaIdRow = await _chartRepositorySeed.GetStationIdAsync("Измайловская", MoscowBranchArbatPokrovIdRow.id);
        MoscowStationPartizanskayaIdRow = await _chartRepositorySeed.GetStationIdAsync("Партизанская", MoscowBranchArbatPokrovIdRow.id);
        MoscowStationSemenovskayaIdRow = await _chartRepositorySeed.GetStationIdAsync("Семёновская", MoscowBranchArbatPokrovIdRow.id);
        MoscowStationElectroZavodskayaIdRow = await _chartRepositorySeed.GetStationIdAsync("Электрозаводская", MoscowBranchArbatPokrovIdRow.id);

        MoscowBranchMoscowskoeCentralnoeKolcoIdRow = await _chartRepositorySeed.GetBranchIdAsync("Московское центральное кольцо", ChartMoscowIdRow.id);
        MoscowStationIzmaylovoIdRow = await _chartRepositorySeed.GetStationIdAsync("Измайлово", MoscowBranchMoscowskoeCentralnoeKolcoIdRow.id);

        MoscowTransitionPartizanskayaIzmaylovoIdRow = await _chartRepositorySeed.GetTransitionIdAsync(MoscowStationPartizanskayaIdRow.id, MoscowStationIzmaylovoIdRow.id);

        await _clientRepositorySeed.MakeDutyOfStation(clientId1Row.id, MoscowStationIzmaylovskayaIdRow.id);
        await _clientRepositorySeed.MakeDuty(clientId2Row.id, MoscowStationPartizanskayaIdRow.id, MoscowTransitionPartizanskayaIzmaylovoIdRow.id);
        await _clientRepositorySeed.MakeDutyOfStation(clientId1Row.id, MoscowStationSemenovskayaIdRow.id);

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

        await _chartRepositorySeed.DeleteChartByIdAsync(ChartAdanaIdRow!.id);
        await _chartRepositorySeed.DeleteChartByIdAsync(ChartMoscowIdRow!.id);
        await _chartRepositorySeed.DeleteChartByIdAsync(ChartSanktPeterburgIdRow!.id);

        await _clientRepositorySeed.DeleteAsync(clientId1Row!.id);
        await _clientRepositorySeed.DeleteAsync(clientId2Row!.id);
        await _clientRepositorySeed.DeleteAsync(clientId3Row!.id);
        await _clientRepositorySeed.DeleteAsync(clientId4Row!.id);
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
        MCMC.Route? serviceRoute = await _routeService.SearchRouteAsync(city, chartTitle, branchSrcTitle, stationSrcTitle, branchDstTitle, stationDstTitle, timeStart) ?? throw new OperationCanceledException();
        MCMA.IdRow chartIdRow = await _chartRepository.GetChartIdAsync(city, chartTitle);
        ClientDTO client = new("Jonh", "Aa1234", "John.Tompson@mail.ru", RoleTypeDTO.DUTY);
        MCMA.IdRow clientIdRow = await _clientService.GetClientIdAsync(client.Login, client.Password);
        MCMA.IdRow savedRouteIdRow = await _routeService.SaveRouteAsync(clientIdRow.id, serviceRoute);
        MCMC.Route? savedRouteJson = await _routeRepository.GetByIdAsync(savedRouteIdRow.id);

        MCMC.Chart[] charts = [ChartAdana, ChartMoscow, ChartSanktPeterburg];
        MCMC.Chart testChart = charts.Where(c => c.City == city && c.Title == chartTitle).FirstOrDefault() ?? throw new OperationCanceledException();
        MCMC.Station src = testChart.GetStation(branchSrcTitle, stationSrcTitle) ?? throw new OperationCanceledException();
        MCMC.Station dst = testChart.GetStation(branchDstTitle, stationDstTitle) ?? throw new OperationCanceledException();
        StrategySearchRouteBase searcher = new StrategySearchRouteBFS();
        MCMC.Route route = testChart.Search(src, dst, timeStart, searcher) ?? throw new OperationCanceledException();
        RouteDTO testRoute = DomainDtoConverter.Convert(route);

        Assert.True(chartIdRow.id > 0);
        Assert.True(savedRouteIdRow.id > 0);
        Assert.True(testRoute.Equals(DomainDtoConverter.Convert(serviceRoute)));
        Assert.NotNull(savedRouteJson);
    }

    [Theory]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "Арбатско-Покровская линия", "Измайловская", "Изипайловская", 10, MCMT.AccessType.INACCESSIBLE, "06:30", "01:15")]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "Арбатско-Покровская линия", "Семёновская", "Серьёзная", 1, MCMT.AccessType.INACCESSIBLE, "06:30", "01:15")]
    public async Task updateStationDutyTest(string chartCity, string chartTitle, string branchTitle, string stationTitle, string newTitle, int newOccupancy, MCMT.AccessType newType, string newOpenTime, string newCloseTime)
    {
        TimeOnly _newOpenTime = TimeConverter.FromString(newOpenTime);
        TimeOnly _newCloseTime = TimeConverter.FromString(newCloseTime);
        MCMC.Station newStation = new(newTitle, newOccupancy, newType, _newOpenTime, _newCloseTime);
        MCMA.ChangedRowCount changesRow = await _chartService.UpdateStationAsync(MCMT.RoleType.DUTY, chartCity, chartTitle, branchTitle, stationTitle, newStation);
        Assert.Equal(1, changesRow.count);
    }

    [Theory]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "Арбатско-Покровская линия", "Партизанская", "Московское центральное кольцо", "Измайлово", 10, MCMT.AccessType.INACCESSIBLE, "05:30", "06:30", "01:15")]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "Арбатско-Покровская линия", "Партизанская", "Московское центральное кольцо", "Измайлово", 1, MCMT.AccessType.INACCESSIBLE, "04:30", "07:30", "02:15")]
    public async Task updateTransitionDutyTest(
        string chartCity, string chartTitle, string fromBranchTitle, string fromStationTitle, string toBranchTitle, string toStationTitle,
        int newOccupancy, MCMT.AccessType newType, string newDuration, string newOpenTime, string newCloseTime)
    {
        TimeSpan _newDuration = TimeSpanConverter.FromString(newDuration);
        TimeOnly _newOpenTime = TimeConverter.FromString(newOpenTime);
        TimeOnly _newCloseTime = TimeConverter.FromString(newCloseTime);
        MCMC.Transition newTransition = new(newOccupancy, newType, _newDuration, _newOpenTime, _newCloseTime);
        MCMA.ChangedRowCount changesRow = await _chartService.UpdateTransitionAsync(MCMT.RoleType.DUTY, chartCity, chartTitle, fromBranchTitle, fromStationTitle, toBranchTitle, toStationTitle, newTransition);

        Assert.Equal(1, changesRow.count);
    }

    [Theory]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "Арбатско-Покровская линия", "Измайловская", "Партизанская", "01:30")]
    [InlineData("Москва", "Московский метрополитен (Тестирование)", "Арбатско-Покровская линия", "Партизанская", "Семёновская", "05:30")]
    public async Task updateRailwayDutyTest(string chartCity, string chartTitle, string branchTitle, string fromStationTitle, string toStationTitle, string newDuration)
    {
        TimeSpan _newDuration = TimeSpanConverter.FromString(newDuration);
        MCMC.Railway newRailway = new(_newDuration);
        MCMA.ChangedRowCount changesRow = await _chartService.UpdateRailwayAsync(MCMT.RoleType.DUTY, chartCity, chartTitle, branchTitle, fromStationTitle, toStationTitle, newRailway);

        Assert.Equal(1, changesRow.count);
    }
}