namespace MetroGidTests.DomainAttribsTests;

using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Models;
using MetroGid.Core.Utilities;
using MetroGid.Core.Utilities.Validators.Handlers;
using MetroGid.Core.Utilities.Validators.Interfaces;

public class StationAttribsTests
{
    [Theory]

    [InlineData("01:30", "23:30", "18:00", true)]
    [InlineData("01:30", "23:30", "01:30", true)]
    [InlineData("01:30", "23:30", "23:30", true)]
    [InlineData("01:30", "23:30", "01:31", true)]
    [InlineData("01:30", "23:30", "23:29", true)]

    [InlineData("01:30", "23:30", "00:00", false)]
    [InlineData("01:30", "23:30", "01:29", false)]
    [InlineData("01:30", "23:30", "23:31", false)]

    [InlineData("05:30", "01:30", "10:30", true)]
    [InlineData("05:30", "01:30", "05:30", true)]
    [InlineData("05:30", "01:30", "01:30", true)]
    [InlineData("05:30", "01:30", "05:31", true)]
    [InlineData("05:30", "01:30", "01:29", true)]

    [InlineData("05:30", "01:30", "02:30", false)]
    [InlineData("05:30", "01:30", "01:31", false)]
    [InlineData("05:30", "01:30", "05:29", false)]
    public void IsOpenedStationTest(string openTimeStr, string closeTimeStr, string curTimeStr, bool opened)
    {
        TimeOnly openTime = TimeConverter.FromString(openTimeStr);
        TimeOnly closeTime = TimeConverter.FromString(closeTimeStr);
        TimeOnly curTime = TimeConverter.FromString(curTimeStr);
        Station station = new("abc", 5, AccessType.ACCESSIBLE, openTime, closeTime);
        bool res = station.IsOpenAt(curTime);
        Assert.Equal(res, opened);
    }





    [Theory]
    [InlineData(AccessType.ACCESSIBLE, true)]
    [InlineData(AccessType.INACCESSIBLE, false)]
    public void IsAccessibleTest(AccessType type, bool accessible)
    {
        Station station = new("abc", 5, type, new TimeOnly(1, 30), new TimeOnly(23, 30));
        bool res = station.IsAccessible();
        Assert.Equal(res, accessible);
    }





    [Theory]
    [InlineData("", 5, 0, "05:30", "01:30", "Наименование станции пустое", ExceptionType.Warning, ExceptionReason.EmptyString)]
    [InlineData(
        "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345",
        5, 0, "05:30", "01:30", "Наименование станции превышает допустимый предел длины: 256 > 255", ExceptionType.Warning, ExceptionReason.StringLenghtOutOfRange)
    ]
    [InlineData("Измайловская", -1, 0, "05:30", "01:30", "Уровень загруженности станции -1 не принадлежит отрезку: [0, 10]", ExceptionType.Warning, ExceptionReason.ValueOutOfRange)]
    [InlineData("Измайловская", 11, 0, "05:30", "01:30", "Уровень загруженности станции 11 не принадлежит отрезку: [0, 10]", ExceptionType.Warning, ExceptionReason.ValueOutOfRange)]
    [InlineData("Измайловская", 5, 5, "05:30", "01:30", "Тип доступа станции 5 не принадлежит перечислению AccessType", ExceptionType.Warning, ExceptionReason.ValueOutOfRange)]
    [InlineData("Измайловская", 5, 0, "05:30", "05:30", "Время открытия и закрытия станции совпадают: 05:30", ExceptionType.Warning, ExceptionReason.NotLogicValue)]
    public void AttribsValidatorTest(string title, int occupancy, int type, string openTimeStr, string closeTimeStr, string exMessege, ExceptionType exType, ExceptionReason exReason)
    {
        TimeOnly openTime = TimeConverter.FromString(openTimeStr);
        TimeOnly closeTime = TimeConverter.FromString(closeTimeStr);
        Station station = new(title, occupancy, (AccessType)type, openTime, closeTime);

        SuperHandlerException handler = new PassThroughHandlerException();
        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        var ex = Assert.Throws<DomainValidationException>(() => { station.Validate(domainAttribsValidator); });

        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }
}