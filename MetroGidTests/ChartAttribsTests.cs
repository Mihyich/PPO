namespace MetroGidTests;

using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Models;
using MetroGid.Core.Utilities.Validators.Handlers;
using MetroGid.Core.Utilities.Validators.Interfaces;

public class ChartAttribsTests
{
    [Theory]
    [InlineData("", "Москва", "svg", "Наименование схемы пустое", ExceptionType.Warning, ExceptionReason.EmptyString)]
    [InlineData(
        "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345",
        "Москва", "svg", "Наименование схемы превышает допустимый предел длины: 256 > 255", ExceptionType.Warning, ExceptionReason.StringLenghtOutOfRange)
    ]
    [InlineData("Московский метрополитен", "", "svg", "Наименование города схемы пустое", ExceptionType.Warning, ExceptionReason.EmptyString)]
    [InlineData(
        "Московский метрополитен",
        "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345",
        "svg", "Наименование города схемы превышает допустимый предел длины: 256 > 255", ExceptionType.Warning, ExceptionReason.StringLenghtOutOfRange)
    ]
    public void AttribsValidatorTest(string title, string city, string svg_inst, string exMessege, ExceptionType exType, ExceptionReason exReason)
    {
        Chart station = new(title, city, svg_inst);

        SuperHandlerException handler = new PassThroughHandlerException();
        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        var ex = Assert.Throws<DomainValidationException>(() => { station.Validate(domainAttribsValidator); });

        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }
}