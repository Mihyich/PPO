using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utility;
using MetroGid.Core.Utility.Builders;
using MetroGid.Core.Utility.Directors;
using MetroGid.Core.Utility.Validators.Handlers;
using MetroGid.Core.Utility.Validators.Interfaces;

namespace MetroGidUnitTests.DomainReferentialityTests;

public class RailwayReferentialityTests
{
    [Fact]
    public void UnlinkedPrevRailwayTest()
    {
        SuperExceptionHandler handler = new PassThroughHandlerException();
        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        IDomainValidatorVisitor domainReferentialityValidator = new ThrowableDomainReferentialityValidator(handler);
        BuilderChart builder = new(domainAttribsValidator, domainReferentialityValidator);
        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string filePath = Path.Combine(currentDirectory, "Cities", "Moscow", "chart.json");
        DirectorChartJson director = new(builder, FileReader.ReadAll(filePath));
        Chart chart = director.Construct();
        Branch branch = chart.Branches[chart.Branches.Count / 2];
        Station station = branch.Stations[chart.Branches[chart.Branches.Count / 2].Stations.Count / 2];
        Railway prevRailway = station?.Prev ?? throw new Exception();
        prevRailway.Prev = null;

        var ex = Assert.Throws<DomainValidationException>(() => { station.Validate(domainReferentialityValidator); });

        Assert.Equal($"Переезд (Prev) со станции '{station.Title}' ветки '{branch.Title}' схемы '{chart.Title}' в городе '{chart.City}' не имеет Prev ссылки", ex.Message);
        Assert.Equal(ExceptionType.Error, ex.ExcType);
        Assert.Equal(ExceptionReason.NullArgument, ex.ExcReason);
    }

    [Fact]
    public void UnlinkedNextRailwayTest()
    {
        SuperExceptionHandler handler = new PassThroughHandlerException();
        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        IDomainValidatorVisitor domainReferentialityValidator = new ThrowableDomainReferentialityValidator(handler);
        BuilderChart builder = new(domainAttribsValidator, domainReferentialityValidator);
        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string filePath = Path.Combine(currentDirectory, "Cities", "Moscow", "chart.json");
        DirectorChartJson director = new(builder, FileReader.ReadAll(filePath));
        Chart chart = director.Construct();
        Branch branch = chart.Branches[chart.Branches.Count / 2];
        Station station = branch.Stations[chart.Branches[chart.Branches.Count / 2].Stations.Count / 2];
        Railway nextRailway = station?.Next ?? throw new Exception();
        nextRailway.Next = null;

        var ex = Assert.Throws<DomainValidationException>(() => { station.Validate(domainReferentialityValidator); });

        Assert.Equal($"Переезд (Next) со станции '{station.Title}' ветки '{branch.Title}' схемы '{chart.Title}' в городе '{chart.City}' не имеет Next ссылки", ex.Message);
        Assert.Equal(ExceptionType.Error, ex.ExcType);
        Assert.Equal(ExceptionReason.NullArgument, ex.ExcReason);
    }
}