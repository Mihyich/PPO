namespace MetroGidTests.DomainAttribsTests;

using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utilities;
using MetroGid.Core.Utilities.Validators.Handlers;
using MetroGid.Core.Utilities.Validators.Interfaces;

public class RailwayAttribsTests
{
    [Theory]
    [InlineData("00:30:01", "Среднее время переезда слишком велико: 00:30", ExceptionType.Warning, ExceptionReason.NotLogicValue)]
    public void AttribsValidatorTest(string durationStr, string exMessege, ExceptionType exType, ExceptionReason exReason)
    {
        TimeOnly duration = TimeConverter.FromString(durationStr);
        Railway railway = new(duration);

        SuperHandlerException handler = new PassThroughHandlerException();
        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        var ex = Assert.Throws<DomainValidationException>(() => { railway.Validate(domainAttribsValidator); });

        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }
}