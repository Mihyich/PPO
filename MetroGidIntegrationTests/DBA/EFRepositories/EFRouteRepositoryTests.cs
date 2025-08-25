using System.Data;
using MetroGid.Core.Interfaces;
using MetroGid.DBA.EF.Context;
using MetroGidIntegrationTests.DBA.EFFixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MCMC = MetroGid.Core.Models.Concrete;
using MCMT = MetroGid.Core.Models.Types;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Utility.Validators.Interfaces;
using MetroGid.Core.Utility.Validators.Handlers;
using MetroGid.Core.Utility.Builders;
using MetroGid.Core.Utility.Directors;
using MetroGid.Core.Utility;
using MetroGid.Core.Utility.Strategies;
using MetroGid.Core.Utility.TimeMeter.Concrete;
using MetroGid.Core.Utility.TimeMeter.Super;
using MetroGid.DBA.EF.Converters;
using Newtonsoft.Json.Linq;
using MetroGid.Core.Converters;

namespace MetroGidIntegrationTests.DBA.EFClientRepositoryTests;

[Collection("Database")]
public class EFRouteRepositoryTests : IClassFixture<EFDataBasePostgresFixture>, IAsyncLifetime
{
    private readonly MetroDbContext _context;
    private readonly IChartRepository _chartRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IRouteRepository _routeRepository;
    private IDbContextTransaction? _transaction;

    private TimeOnly TimeStart = new(9, 0);

    private string ChartJsonAdana;
    private string ChartJsonMoscow;
    private string ChartJsonSanktPeterburg;

    private MCMC.Chart ChartAdana;
    private MCMC.Chart ChartMoscow;
    private MCMC.Chart ChartSanktPeterburg;

    private MCMC.Route Route_Adana_Bolnitsa_Akindjilar;

    private MCMC.Route Route_Moscow_Izmaylovskaya_Baumanskaya;
    private MCMC.Route Route_Moscow_Nahabino_Ipodrom;
    private MCMC.Route Route_Moscow_Sviblovo_Fili;

    private MCMC.Route Route_Sankt_Peterburg_Begovaya_Kupchino;

    private int clientId1;
    private int clientId2;
    private int clientId3;
    private int clientId4;

    private int ChartAdanaId;
    private int ChartMoscowId;
    private int ChartSanktPeterburgId;

    public EFRouteRepositoryTests(EFDataBasePostgresFixture fixture)
    {
        _context = fixture.Context;
        _chartRepository = fixture.chartRepository;
        _clientRepository = fixture.clientRepository;
        _routeRepository = fixture.routeRepository;

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

        TimeSuper timeMeter = new TimeFast();
        StrategySearchRouteBase searcher = new StrategySearchRouteDijkstra(timeMeter);

        MCMC.Station src;
        MCMC.Station dst;

        src = ChartAdana.GetStation("Линия 1", "Больница") ?? throw new InvalidOperationException("Станция не найдена");
        dst = ChartAdana.GetStation("Линия 1", "Акынджилар") ?? throw new InvalidOperationException("Станция не найдена");
        Route_Adana_Bolnitsa_Akindjilar = ChartAdana.Search(src, dst, TimeStart, searcher) ?? throw new InvalidOperationException("Маршрут не найден");

        src = ChartMoscow.GetStation("Арбатско-Покровская линия", "Измайловская") ?? throw new InvalidOperationException("Станция не найдена");
        dst = ChartMoscow.GetStation("Арбатско-Покровская линия", "Бауманская") ?? throw new InvalidOperationException("Станция не найдена");
        Route_Moscow_Izmaylovskaya_Baumanskaya = ChartMoscow.Search(src, dst, TimeStart, searcher) ?? throw new InvalidOperationException("Маршрут не найден");

        src = ChartMoscow.GetStation("МЦД-2", "Нахабино") ?? throw new InvalidOperationException("Станция не найдена");
        dst = ChartMoscow.GetStation("МЦД-3", "Ипподром") ?? throw new InvalidOperationException("Станция не найдена");
        Route_Moscow_Nahabino_Ipodrom = ChartMoscow.Search(src, dst, TimeStart, searcher) ?? throw new InvalidOperationException("Маршрут не найден");

        src = ChartMoscow.GetStation("Калужско-Рижская линия", "Свиблово") ?? throw new InvalidOperationException("Станция не найдена");
        dst = ChartMoscow.GetStation("Филёвская линия", "Фили") ?? throw new InvalidOperationException("Станция не найдена");
        Route_Moscow_Sviblovo_Fili = ChartMoscow.Search(src, dst, TimeStart, searcher) ?? throw new InvalidOperationException("Маршрут не найден");

        src = ChartSanktPeterburg.GetStation("Невско-Василеостровская", "Беговая") ?? throw new InvalidOperationException("Станция не найдена");
        dst = ChartSanktPeterburg.GetStation("Московско-Петроградская", "Купчино") ?? throw new InvalidOperationException("Станция не найдена");
        Route_Sankt_Peterburg_Begovaya_Kupchino = ChartSanktPeterburg.Search(src, dst, TimeStart, searcher) ?? throw new InvalidOperationException("Маршрут не найден");
    }

    public async Task InitializeAsync()
    {
        if (_context.Database.GetDbConnection().State != ConnectionState.Open)
            await _context.Database.OpenConnectionAsync();

        _transaction = await _context.Database.BeginTransactionAsync();

        clientId1 = await _clientRepository.AddAsync(new("TestUser1", "Aa1234", "Test.User.1@test.ru", MCMT.RoleType.SIGNED));
        clientId2 = await _clientRepository.AddAsync(new("TestUser2", "Aa1234", "Test.User.2@test.ru", MCMT.RoleType.SIGNED));
        clientId3 = await _clientRepository.AddAsync(new("TestUser3", "Aa1234", "Test.User.3@test.ru", MCMT.RoleType.SIGNED));
        clientId4 = await _clientRepository.AddAsync(new("TestUser4", "Aa1234", "Test.User.4@test.ru", MCMT.RoleType.SIGNED));

        ChartAdanaId = await _chartRepository.AddAsync(ChartJsonAdana);
        ChartMoscowId = await _chartRepository.AddAsync(ChartJsonMoscow);
        ChartSanktPeterburgId = await _chartRepository.AddAsync(ChartJsonSanktPeterburg);

        await _routeRepository.AddAsync(clientId1, ChartMoscowId, DtoRouteJsonConverter.Convert(DomainDtoConverter.Convert(Route_Moscow_Izmaylovskaya_Baumanskaya)));
        await _routeRepository.AddAsync(clientId1, ChartMoscowId, DtoRouteJsonConverter.Convert(DomainDtoConverter.Convert(Route_Moscow_Sviblovo_Fili)));

        await _routeRepository.AddAsync(clientId2, ChartAdanaId, DtoRouteJsonConverter.Convert(DomainDtoConverter.Convert(Route_Adana_Bolnitsa_Akindjilar)));

        await _routeRepository.AddAsync(clientId3, ChartAdanaId, DtoRouteJsonConverter.Convert(DomainDtoConverter.Convert(Route_Adana_Bolnitsa_Akindjilar)));
        await _routeRepository.AddAsync(clientId3, ChartMoscowId, DtoRouteJsonConverter.Convert(DomainDtoConverter.Convert(Route_Moscow_Izmaylovskaya_Baumanskaya)));
        await _routeRepository.AddAsync(clientId3, ChartMoscowId, DtoRouteJsonConverter.Convert(DomainDtoConverter.Convert(Route_Moscow_Nahabino_Ipodrom)));
        await _routeRepository.AddAsync(clientId3, ChartMoscowId, DtoRouteJsonConverter.Convert(DomainDtoConverter.Convert(Route_Moscow_Sviblovo_Fili)));
        await _routeRepository.AddAsync(clientId3, ChartSanktPeterburgId, DtoRouteJsonConverter.Convert(DomainDtoConverter.Convert(Route_Sankt_Peterburg_Begovaya_Kupchino)));

        await _context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }
    }

    [Theory]
    [InlineData("TestUser1", "Aa1234", "Test.User.1@test.ru")]
    [InlineData("TestUser2", "Aa1234", "Test.User.2@test.ru")]
    [InlineData("TestUser3", "Aa1234", "Test.User.3@test.ru")]
    [InlineData("TestUser4", "Aa1234", "Test.User.4@test.ru")]
    public async Task InitializeAsyncTest(string login, string password, string mail)
    {
        Assert.NotNull(await _clientRepository.GetByCredentialsAsync(login, password, mail));
    }

    [Fact]
    public async Task GetAsyncTest()
    {
        var testData = new[]
        {
            (Route_Moscow_Izmaylovskaya_Baumanskaya, clientId1),
            (Route_Moscow_Sviblovo_Fili, clientId1),

            (Route_Adana_Bolnitsa_Akindjilar, clientId2),

            (Route_Adana_Bolnitsa_Akindjilar, clientId3),
            (Route_Moscow_Izmaylovskaya_Baumanskaya, clientId3),
            (Route_Moscow_Nahabino_Ipodrom, clientId3),
            (Route_Moscow_Sviblovo_Fili, clientId3),
            (Route_Sankt_Peterburg_Begovaya_Kupchino, clientId3)
        };

        foreach (var (route, clientId) in testData)
        {
            int routeId = await _routeRepository.GetIdAsync(route.Title, clientId);
            string? routeJson = await _routeRepository.GetByIdAsync(routeId);

            Assert.NotNull(routeJson);
            Assert.True(JToken.DeepEquals(
                JToken.Parse(routeJson),
                JToken.Parse(DomainRouteJsonConverter.Convert(route))
            ));
        }
    }
}