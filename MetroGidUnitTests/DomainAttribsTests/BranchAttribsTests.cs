using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Models.Types;
using MetroGid.Core.Utilities.Validators.Handlers;
using MetroGid.Core.Utilities.Validators.Interfaces;

namespace MetroGidUnitTests.DomainAttribsTests;

public class BranchAttribsTests
{
    [Theory]
    [InlineData("", 0, 0, "Наименование ветки пустое", ExceptionType.Warning, ExceptionReason.EmptyString)]
    [InlineData(
        "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345",
        0, 0, "Наименование ветки превышает допустимый предел длины: 256 > 255", ExceptionType.Warning, ExceptionReason.StringLenghtOutOfRange)
    ]
    [InlineData("Арбатско-Покровская", -1, 0, "10-ое представление значения цвета ветки -1 не принадлежит отрезку: [0, 16777215]", ExceptionType.Warning, ExceptionReason.ValueOutOfRange)]
    [InlineData("Арбатско-Покровская", 16777216, 0, "10-ое представление значения цвета ветки 16777216 не принадлежит отрезку: [0, 16777215]", ExceptionType.Warning, ExceptionReason.ValueOutOfRange)]
    [InlineData("Арбатско-Покровская", 0, 5, "Тип доступа ветки 5 не принадлежит перечислению AccessType", ExceptionType.Warning, ExceptionReason.ValueOutOfRange)]
    public void AttribsValidatorTest(string title, int color, int type, string exMessege, ExceptionType exType, ExceptionReason exReason)
    {
        Branch branch = new(title, color, (AccessType)type);

        SuperExceptionHandler handler = new PassThroughHandlerException();
        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        var ex = Assert.Throws<DomainValidationException>(() => { branch.Validate(domainAttribsValidator); });

        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }
}