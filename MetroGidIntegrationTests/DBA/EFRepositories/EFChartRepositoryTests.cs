using System.Data;
using MCMC = MetroGid.Core.Models.Concrete;
using MCMA = MetroGid.Core.Models.Advanced;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Interfaces;
using MetroGid.Core.Utility;
using MetroGid.Core.Utility.Builders;
using MetroGid.Core.Utility.Directors;
using MetroGid.Core.Utility.Validators.Handlers;
using MetroGid.Core.Utility.Validators.Interfaces;
using MetroGid.DBA.EF.Context;
using MetroGidIntegrationTests.DBA.EFFixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json.Linq;
using FluentAssertions;

namespace MetroGidIntegrationTests.DBA.EFClientRepositoryTests;

[Collection("Database")]
public class EFChartRepositoryTests : IClassFixture<EFDataBasePostgresFixture>, IAsyncLifetime
{
    private readonly MetroDbContext _context;
    private readonly IChartRepository _chartRepository;
    private IDbContextTransaction? _transaction;

    private string ChartJsonAdana;
    private string ChartJsonMoscow;
    private string ChartJsonSanktPeterburg;

    private MCMC.Chart ChartAdana;
    private MCMC.Chart ChartMoscow;
    private MCMC.Chart ChartSanktPeterburg;

    private MCMA.IdRow? ChartAdanaIdRow;
    private MCMA.IdRow? ChartMoscowIdRow;
    private MCMA.IdRow? ChartSanktPeterburgIdRow;

    public EFChartRepositoryTests(EFDataBasePostgresFixture fixture)
    {
        _context = fixture.Context;
        _chartRepository = fixture.chartRepository;

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
        if (_context.Database.GetDbConnection().State != ConnectionState.Open)
            await _context.Database.OpenConnectionAsync();

        _transaction = await _context.Database.BeginTransactionAsync();

        ChartAdanaIdRow = await _chartRepository.AddAsync(ChartJsonAdana);
        ChartMoscowIdRow = await _chartRepository.AddAsync(ChartJsonMoscow);
        ChartSanktPeterburgIdRow = await _chartRepository.AddAsync(ChartJsonSanktPeterburg);

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

    [Fact]
    public async Task getAsyncTest()
    {
        var testData = new[]
        {
            (ChartAdanaIdRow, ChartAdana.City, ChartAdana.Title, ChartJsonAdana),
            (ChartMoscowIdRow, ChartMoscow.City, ChartMoscow.Title, ChartJsonMoscow),
            (ChartSanktPeterburgIdRow, ChartSanktPeterburg.City, ChartSanktPeterburg.Title, ChartJsonSanktPeterburg)
        };

        foreach (var (chartId, city, title, expectedChartJson) in testData)
        {
            MCMA.IdRow testChartIdRow = await _chartRepository.GetChartIdAsync(city, title);
            MCMA.FileRow? chartJsonRow = await _chartRepository.GetChartJsonByIdAsync(testChartIdRow.id);

            Assert.NotNull(chartJsonRow);
            Assert.Equal(chartId, testChartIdRow);
            JToken.Parse(chartJsonRow.content).Should().BeEquivalentTo(JToken.Parse(expectedChartJson));
        }
    }
}