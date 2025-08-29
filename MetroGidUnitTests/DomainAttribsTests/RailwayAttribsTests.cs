using MetroGid.Core.Converters;
using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utility.Validators.Handlers;
using MetroGid.Core.Utility.Validators.Interfaces;

namespace MetroGidUnitTests.DomainAttribsTests;

public class RailwayAttribsTests
{
    [Theory]
    [InlineData("00:30:01", "Среднее время переезда слишком велико: 00:30:01", ExceptionType.Warning, ExceptionReason.NotLogicValue)]
    public void AttribsValidatorTest(string durationStr, string exMessege, ExceptionType exType, ExceptionReason exReason)
    {
        TimeSpan duration = TimeSpanConverter.FromString(durationStr);
        Railway railway = new(duration);

        SuperExceptionHandler handler = new PassThroughHandlerException();
        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        var ex = Assert.Throws<DomainValidationException>(() => { railway.Validate(domainAttribsValidator); });

        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }
}