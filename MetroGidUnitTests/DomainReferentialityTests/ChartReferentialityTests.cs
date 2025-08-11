using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utilities;
using MetroGid.Core.Utilities.Builders;
using MetroGid.Core.Utilities.Directors;
using MetroGid.Core.Utilities.Validators.Handlers;
using MetroGid.Core.Utilities.Validators.Interfaces;

namespace MetroGidUnitTests.DomainReferentialityTests;

public class ChartReferentialityTests
{
    [Fact]
    public void NoBranchesTest()
    {
        SuperExceptionHandler handler = new PassThroughHandlerException();
        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        IDomainValidatorVisitor domainReferentialityValidator = new ThrowableDomainReferentialityValidator(handler);
        BuilderChart builder = new(domainAttribsValidator, domainReferentialityValidator);
        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string filePath = Path.Combine(currentDirectory, "Cities", "Adana", "chart.json");
        DirectorChartJson director = new(builder, FileReader.ReadAll(filePath));
        Chart chart = director.Construct();
        chart.Branches.Clear();

        var ex = Assert.Throws<DomainValidationException>(() => { chart.Validate(domainReferentialityValidator); });

        Assert.Equal($"Схема '{chart.Title}' в городе '{chart.City}' не имеет ни одной ветки", ex.Message);
        Assert.Equal(ExceptionType.Warning, ex.ExcType);
        Assert.Equal(ExceptionReason.NotFound, ex.ExcReason);
    }

    [Fact]
    public void DuplicatedBranchTitlesTest()
    {
        SuperExceptionHandler handler = new PassThroughHandlerException();
        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        IDomainValidatorVisitor domainReferentialityValidator = new ThrowableDomainReferentialityValidator(handler);
        BuilderChart builder = new(domainAttribsValidator, domainReferentialityValidator);
        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string filePath = Path.Combine(currentDirectory, "Cities", "Moscow", "chart.json");
        DirectorChartJson director = new(builder, FileReader.ReadAll(filePath));
        Chart chart = director.Construct();
        chart.Branches[chart.Branches.Count / 2].Title = chart.Branches[^1].Title;

        var ex = Assert.Throws<DomainValidationException>(() => { chart.Validate(domainReferentialityValidator); });

        Assert.Equal($"Схема '{chart.Title}' в городе '{chart.City}' имеет ветки с одинаковыми наименованиями: '{chart.Branches[chart.Branches.Count / 2].Title}'", ex.Message);
        Assert.Equal(ExceptionType.Error, ex.ExcType);
        Assert.Equal(ExceptionReason.ValueDuplicate, ex.ExcReason);
    }

    [Fact]
    public void UnlinkedBranchTest()
    {
        SuperExceptionHandler handler = new PassThroughHandlerException();
        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        IDomainValidatorVisitor domainReferentialityValidator = new ThrowableDomainReferentialityValidator(handler);
        BuilderChart builder = new(domainAttribsValidator, domainReferentialityValidator);
        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string filePath = Path.Combine(currentDirectory, "Cities", "Moscow", "chart.json");
        DirectorChartJson director = new(builder, FileReader.ReadAll(filePath));
        Chart chart = director.Construct();
        chart.Branches[chart.Branches.Count / 2].Chart = null;

        var ex = Assert.Throws<DomainValidationException>(() => { chart.Validate(domainReferentialityValidator); });

        Assert.Equal($"Ветка '{chart.Branches[chart.Branches.Count / 2].Title}' схемы '{chart.Title}' в городе '{chart.City}' не привязана к родной схеме", ex.Message);
        Assert.Equal(ExceptionType.Error, ex.ExcType);
        Assert.Equal(ExceptionReason.NullArgument, ex.ExcReason);
    }

    [Fact]
    public void MismatchlinkingBranchTest()
    {
        SuperExceptionHandler handler = new PassThroughHandlerException();
        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        IDomainValidatorVisitor domainReferentialityValidator = new ThrowableDomainReferentialityValidator(handler);
        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string filePath1 = Path.Combine(currentDirectory, "Cities", "Moscow", "chart.json");
        string filePath2 = Path.Combine(currentDirectory, "Cities", "Sankt-Peterburg", "chart.json");
        Chart chart1 = new DirectorChartJson(new BuilderChart(domainAttribsValidator, domainReferentialityValidator), FileReader.ReadAll(filePath1)).Construct();
        Chart chart2 = new DirectorChartJson(new BuilderChart(domainAttribsValidator, domainReferentialityValidator), FileReader.ReadAll(filePath2)).Construct();
        chart1.Branches[chart1.Branches.Count / 2].Chart = chart2;

        var ex = Assert.Throws<DomainValidationException>(() => { chart1.Validate(domainReferentialityValidator); });

        Assert.Equal($"Ветка '{chart1.Branches[chart1.Branches.Count / 2].Title}' схемы '{chart1.Title}' в городе '{chart1.City}' ссылается не на родную схему", ex.Message);
        Assert.Equal(ExceptionType.Error, ex.ExcType);
        Assert.Equal(ExceptionReason.IncorrectLink, ex.ExcReason);
    }
}