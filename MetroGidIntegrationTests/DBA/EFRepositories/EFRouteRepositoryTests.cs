using System.Data;
using MetroGid.Core.Interfaces;
using MCMC = MetroGid.Core.Models.Types;
using MetroGid.DBA.EF.Context;
using MetroGidIntegrationTests.DBA.EFFixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Loggers;
using MetroGid.Core.Utilities.Validators.Interfaces;
using MetroGid.Core.Utilities.Validators.Handlers;
using MetroGid.Core.Utilities.Builders;
using MetroGid.Core.Utilities.Directors;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utilities;
using MetroGid.Core.Utilities.Strategies;
using MetroGid.Core.Utilities.TimeMeter.Concrete;
using MetroGid.Core.Utilities.TimeMeter.Super;

namespace MetroGidIntegrationTests.DBA.EFClientRepositoryTests;

[Collection("Database")]
public class EFRouteRepositoryTests : IClassFixture<EFDataBaseFixture>, IAsyncLifetime
{
    private readonly MetroDbContext _context;
    private readonly IClientRepository _clientRepository;
    private readonly IRouteRepository _routeRepository;
    private IDbContextTransaction? _transaction;


    private TimeOnly TimeStart = new(9, 0);

    private Chart ChartAdana;
    private Chart ChartMoscow;
    private Chart ChartSanktPeterburg;


    private Route Route_Adana_Bolnitsa_Akindjilar;


    private Route Route_Moscow_Izmaylovskaya_Baumanskaya;
    private Route Route_Moscow_Nahabino_Ipodrom;
    private Route Route_Moscow_Sviblovo_Fili;


    private Route Route_Sankt_Peterburg_Begovaya_Kupchino;


    public EFRouteRepositoryTests(EFDataBaseFixture fixture)
    {
        _context = fixture.Context;
        _clientRepository = fixture.clientRepository;
        _routeRepository = fixture.routeRepository;

        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string AdanaChartPath = Path.Combine(currentDirectory, "Cities", "Adana", "chart.json");
        string MoscowChartPath = Path.Combine(currentDirectory, "Cities", "Moscow", "chart.json");
        string SanktPeterburgChartPath = Path.Combine(currentDirectory, "Cities", "Sankt-Peterburg", "chart.json");

        SuperExceptionHandler handler = new WarningHandlerException();
        IExceptionVisitor logger = new ExceptionMessenger();

        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler, logger);
        IDomainValidatorVisitor domainReferentialityValidator = new ThrowableDomainReferentialityValidator(handler, logger);

        BuilderChartBase builder;
        DirectorChartBase director;

        builder = new BuilderChart(domainAttribsValidator, domainReferentialityValidator);
        director = new DirectorChartJson(builder, FileReader.ReadAll(AdanaChartPath));
        ChartAdana = director.Construct();

        builder = new BuilderChart(domainAttribsValidator, domainReferentialityValidator);
        director = new DirectorChartJson(builder, FileReader.ReadAll(MoscowChartPath));
        ChartMoscow = director.Construct();

        builder = new BuilderChart(domainAttribsValidator, domainReferentialityValidator);
        director = new DirectorChartJson(builder, FileReader.ReadAll(SanktPeterburgChartPath));
        ChartSanktPeterburg = director.Construct();

        TimeSuper timeMeter = new TimeFast();
        StrategySearchRouteBase searcher = new StrategySearchRouteBFS(timeMeter);

        Station src;
        Station dst;

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

        int clientId1 = await _clientRepository.AddAsync(new("TestUser1", "Aa1234", "Test.User.1@test.ru", MCMC.RoleType.SIGNED));
        int clientId2 = await _clientRepository.AddAsync(new("TestUser2", "Aa1234", "Test.User.2@test.ru", MCMC.RoleType.SIGNED));
        int clientId3 = await _clientRepository.AddAsync(new("TestUser3", "Aa1234", "Test.User.3@test.ru", MCMC.RoleType.SIGNED));
        int clientId4 = await _clientRepository.AddAsync(new("TestUser4", "Aa1234", "Test.User.4@test.ru", MCMC.RoleType.SIGNED));

        // await _routeRepository.AddAsync(clientId1, )

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

    
}