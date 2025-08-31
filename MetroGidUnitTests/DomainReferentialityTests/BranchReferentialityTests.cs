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

public class BranchReferentialityTests
{
    [Fact]
    public void NoStationsTest()
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
        branch.Stations.Clear();

        var ex = Assert.Throws<DomainValidationException>(() => { chart.Validate(domainReferentialityValidator); });

        Assert.Equal($"Ветка '{branch.Title}' схемы '{chart.Title}' в городе '{chart.City}' не имеет ни одной станции", ex.Message);
        Assert.Equal(ExceptionType.Warning, ex.ExcType);
        Assert.Equal(ExceptionReason.NotFound, ex.ExcReason);
    }

    [Fact]
    public void DuplicatedStationTitlesTest()
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
        Station station = branch.Stations[branch.Stations.Count / 2];
        station.Title = branch.Stations[^1].Title;

        var ex = Assert.Throws<DomainValidationException>(() => { chart.Validate(domainReferentialityValidator); });

        Assert.Equal($"Ветка схемы '{chart.Title}' имеет станции с одинаковыми наименованиями: '{station.Title}'", ex.Message);
        Assert.Equal(ExceptionType.Error, ex.ExcType);
        Assert.Equal(ExceptionReason.ValueDuplicate, ex.ExcReason);
    }

    [Fact]
    public void UnlinkedStationTest()
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
        station.Branch = null;

        var ex = Assert.Throws<DomainValidationException>(() => { chart.Validate(domainReferentialityValidator); });

        Assert.Equal($"Станция '{station.Title}' ветки '{branch.Title}' схемы '{chart.Title}' в городе '{chart.City}' не привязана к родной ветке", ex.Message);
        Assert.Equal(ExceptionType.Error, ex.ExcType);
        Assert.Equal(ExceptionReason.NullArgument, ex.ExcReason);
    }

    [Fact]
    public void MismatchlinkingStationTest()
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
        station.Branch = chart.Branches[^1];

        var ex = Assert.Throws<DomainValidationException>(() => { chart.Validate(domainReferentialityValidator); });

        Assert.Equal($"Станция '{station.Title}' ветки '{branch.Title}' схемы '{chart.Title}' в городе '{chart.City}' ссылается не на родную ветку", ex.Message);
        Assert.Equal(ExceptionType.Error, ex.ExcType);
        Assert.Equal(ExceptionReason.IncorrectLink, ex.ExcReason);
    }
}