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

public class StationReferentialityTests
{
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

        var ex = Assert.Throws<DomainValidationException>(() => { station.Validate(domainReferentialityValidator); });

        Assert.Equal($"Станция '{station.Title}' ветки 'Неизвестно' схемы 'Неизвестно' в городе 'Неизвестно' не связана с родной веткой", ex.Message);
        Assert.Equal(ExceptionType.Error, ex.ExcType);
        Assert.Equal(ExceptionReason.NullArgument, ex.ExcReason);
    }

    [Fact]
    public void UnLinkiedPrevStaionTest()
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
        Station prevStation = station.Prev?.Prev ?? throw new Exception();
        prevStation.Branch = null;

        var ex = Assert.Throws<DomainValidationException>(() => { station.Validate(domainReferentialityValidator); });

        Assert.Equal($"Станция '{station.Title}' ветки '{branch.Title}' схемы '{chart.Title}' в городе '{chart.City}' ведет на станцию '{prevStation.Title}' не связанной с родной веткой", ex.Message);
        Assert.Equal(ExceptionType.Error, ex.ExcType);
        Assert.Equal(ExceptionReason.NullArgument, ex.ExcReason);
    }

    [Fact]
    public void MismatchLinkingPrevStaionTest()
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
        Station prevStation = station.Prev?.Prev ?? throw new Exception();
        prevStation.Branch = chart.Branches[^1];

        var ex = Assert.Throws<DomainValidationException>(() => { station.Validate(domainReferentialityValidator); });

        Assert.Equal($"Станция '{station.Title}' ветки '{branch.Title}' схемы '{chart.Title}' в городе '{chart.City}' ведет на станцию '{prevStation.Title}' связанной с другой веткой", ex.Message);
        Assert.Equal(ExceptionType.Error, ex.ExcType);
        Assert.Equal(ExceptionReason.IncorrectLink, ex.ExcReason);
    }

    [Fact]
    public void UnLinkiedNextStaionTest()
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
        Station nextStation = station.Next?.Next ?? throw new Exception();
        nextStation.Branch = null;

        var ex = Assert.Throws<DomainValidationException>(() => { station.Validate(domainReferentialityValidator); });

        Assert.Equal($"Станция '{station.Title}' ветки '{branch.Title}' схемы '{chart.Title}' в городе '{chart.City}' ведет на станцию '{nextStation.Title}' не связанной с родной веткой", ex.Message);
        Assert.Equal(ExceptionType.Error, ex.ExcType);
        Assert.Equal(ExceptionReason.NullArgument, ex.ExcReason);
    }

    [Fact]
    public void MismatchLinkingNextStaionTest()
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
        Station nextStation = station.Next?.Next ?? throw new Exception();
        nextStation.Branch = chart.Branches[^1];

        var ex = Assert.Throws<DomainValidationException>(() => { station.Validate(domainReferentialityValidator); });

        Assert.Equal($"Станция '{station.Title}' ветки '{branch.Title}' схемы '{chart.Title}' в городе '{chart.City}' ведет на станцию '{nextStation.Title}' связанной с другой веткой", ex.Message);
        Assert.Equal(ExceptionType.Error, ex.ExcType);
        Assert.Equal(ExceptionReason.IncorrectLink, ex.ExcReason);
    }
}