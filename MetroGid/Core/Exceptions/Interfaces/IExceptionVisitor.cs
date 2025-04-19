using MetroGid.Core.Exceptions.Concrete;

namespace MetroGid.Core.Exceptions.Interfaces
{
    public interface IExceptionVisitor
    {
        void Visit(LoginValidationException ex);
        void Visit(PasswordValidationException ex);
        void Visit(MailValidationException ex);
        void Visit(ItemAlreadyInUseException ex);
        void Visit(NotFoundException ex);
    }
}