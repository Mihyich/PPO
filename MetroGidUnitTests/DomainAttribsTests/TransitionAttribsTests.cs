using MetroGid.Core.Converters;
using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Models.Types;
using MetroGid.Core.Utility.Validators.Handlers;
using MetroGid.Core.Utility.Validators.Interfaces;

namespace MetroGidUnitTests.DomainAttribsTests;

public class TransitionAttribsTests
{
    [Theory]
    [InlineData(-1, 0, "00:03:00", "05:30", "01:30", "Уровень загруженности перехода -1 не принадлежит отрезку: [0, 10]", ExceptionType.Warning, ExceptionReason.ValueOutOfRange)]
    [InlineData(11, 0, "00:03:00", "05:30", "01:30", "Уровень загруженности перехода 11 не принадлежит отрезку: [0, 10]", ExceptionType.Warning, ExceptionReason.ValueOutOfRange)]
    [InlineData(5, 5, "00:03:00", "05:30", "01:30", "Тип доступа перехода 5 не принадлежит перечислению AccessType", ExceptionType.Warning, ExceptionReason.ValueOutOfRange)]
    [InlineData(5, 0, "01:01:00", "01:30", "01:30", "Среднее время перехода слишком велико: 01:01", ExceptionType.Warning, ExceptionReason.NotLogicValue)]
    [InlineData(5, 0, "00:03:00", "01:30", "01:30", "Время открытия и закрытия перехода совпадают: 01:30", ExceptionType.Warning, ExceptionReason.NotLogicValue)]
    public void AttribsValidatorTest(int occupancy, int type, string durationStr, string openTimeStr, string closeTimeStr, string exMessege, ExceptionType exType, ExceptionReason exReason)
    {
        TimeOnly duration = TimeConverter.FromString(durationStr);
        TimeOnly openTime = TimeConverter.FromString(openTimeStr);
        TimeOnly closeTime = TimeConverter.FromString(closeTimeStr);
        Transition transition = new(occupancy, (AccessType)type, duration, openTime, closeTime);

        SuperExceptionHandler handler = new PassThroughHandlerException();
        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        var ex = Assert.Throws<DomainValidationException>(() => { transition.Validate(domainAttribsValidator); });

        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }
}