namespace MetroGidTests.DomainReferentialityTests;

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

public class TransitionReferentialityTests
{
    [Fact]
    public void MismatchLinkingFromTransitionTest()
    {
        SuperHandlerException handler = new PassThroughHandlerException();
        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        IDomainValidatorVisitor domainReferentialityValidator = new ThrowableDomainReferentialityValidator(handler);
        BuilderChart builder = new(domainAttribsValidator, domainReferentialityValidator);
        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string filePath = Path.Combine(currentDirectory, "Cities", "Moscow", "chart.json");
        DirectorChartJson director = new(builder, FileReader.ReadAll(filePath));
        Chart chart = director.Construct();
        Branch branch = chart.Branches[chart.Branches.Count / 2];
        Station station = chart?.GetStation("Сокольническая линия", "Лубянка") ?? throw new Exception();
        Transition transition = station.Transitions.Count > 0 ? station.Transitions[0] : throw new Exception();
        transition.From = null;

        var ex = Assert.Throws<DomainValidationException>(() => { transition.Validate(domainReferentialityValidator); });

        Assert.Equal($"Переход ветки '{transition?.To?.Title ?? "Неизвестно"}' схемы '{chart.Title}' в городе '{chart.City}' не имеет From ссылки", ex.Message);
        Assert.Equal(ExceptionType.Error, ex.ExcType);
        Assert.Equal(ExceptionReason.NullArgument, ex.ExcReason);
    }

    [Fact]
    public void MismatchLinkingToTransitionTest()
    {
        SuperHandlerException handler = new PassThroughHandlerException();
        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        IDomainValidatorVisitor domainReferentialityValidator = new ThrowableDomainReferentialityValidator(handler);
        BuilderChart builder = new(domainAttribsValidator, domainReferentialityValidator);
        string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
        string filePath = Path.Combine(currentDirectory, "Cities", "Moscow", "chart.json");
        DirectorChartJson director = new(builder, FileReader.ReadAll(filePath));
        Chart chart = director.Construct();
        Branch branch = chart.Branches[chart.Branches.Count / 2];
        Station station = chart?.GetStation("Таганско-Краснопресненская линия", "Кузнецкий Мост") ?? throw new Exception();
        Transition transition = station.Transitions.Count > 0 ? station.Transitions[0] : throw new Exception();
        transition.To = null;

        var ex = Assert.Throws<DomainValidationException>(() => { transition.Validate(domainReferentialityValidator); });

        Assert.Equal($"Переход ветки '{transition?.From?.Title ?? "Неизвестно"}' схемы '{chart.Title}' в городе '{chart.City}' не имеет To ссылки", ex.Message);
        Assert.Equal(ExceptionType.Error, ex.ExcType);
        Assert.Equal(ExceptionReason.NullArgument, ex.ExcReason);
    }
}