using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utility.Validators.Handlers;
using MetroGid.Core.Utility.Validators.Interfaces;

namespace MetroGidUnitTests.DomainAttribsTests;

public class ClientAttribsTests
{
    [Theory]
    [InlineData("ab1234")]
    [InlineData("123456")]
    [InlineData("A1234A")]
    [InlineData("a1b2c3")]
    [InlineData("A1B2C3")]
    [InlineData("AaBbCc")]
    public void PasswordValidatorTest(string p)
    {
        Client client = new("login", p, "abc@mail.ru");
        string exMessege = $"Пароль клиета '{client.Password}' не валиден";
        ExceptionType exType = ExceptionType.Warning;
        ExceptionReason exReason = ExceptionReason.ValidationFailed;

        SuperExceptionHandler handler = new PassThroughHandlerException();
        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        var ex = Assert.Throws<DomainValidationException>(() => { client.Validate(domainAttribsValidator); });

        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }

    [Theory]
    [InlineData(".a@mail.ru")]
    [InlineData("a.@mail.ru")]
    [InlineData("a..b@mail.ru")]
    [InlineData("abc@.mail.ru")]
    [InlineData("abc@mail.ru.")]
    [InlineData("abc@mail.r")]
    [InlineData("abc@mail.ru-")]
    [InlineData("abc@-mail.ru")]
    public void MailValidatorTest(string m)
    {
        Client client = new("login", "Aa1234", m);
        string exMessege = $"Почта клиета '{client.Mail}' не валидна";
        ExceptionType exType = ExceptionType.Warning;
        ExceptionReason exReason = ExceptionReason.ValidationFailed;

        SuperExceptionHandler handler = new PassThroughHandlerException();
        IDomainValidatorVisitor domainAttribsValidator = new ThrowableDomainAttribsValidator(handler);
        var ex = Assert.Throws<DomainValidationException>(() => { client.Validate(domainAttribsValidator); });

        Assert.Equal(exMessege, ex.Message);
        Assert.Equal(exType, ex.ExcType);
        Assert.Equal(exReason, ex.ExcReason);
    }
}