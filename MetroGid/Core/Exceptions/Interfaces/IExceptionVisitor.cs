using MetroGid.Core.Exceptions.Concrete;

namespace MetroGid.Core.Exceptions.Interfaces
{
    public interface IExceptionVisitor
    {
        void Visit(LoginValidationException ex);
        void Visit(PasswordValidationException ex);
        void Visit(MailValidationException ex);
        void Visit(ItemAlreadyInUseException ex);
        void Visit(JsonDeserializeException ex);
        void Visit(JsonValidationException ex);
        void Visit(BuilderValidationException ex);
        void Visit(BuilderProccessException ex);
        void Visit(DomainValidationException ex);
        void Visit(NotFoundException ex);
        void Visit(ServiceRouteException ex);
    }
}