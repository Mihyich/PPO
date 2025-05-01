using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Exceptions.Interfaces;

namespace MetroGid.Core.Exceptions.Loggers
{
    public class ExceptionMessenger : IExceptionVisitor
    {
        public void Visit(LoginValidationException ex)
        {
            throw new NotImplementedException();
        }

        public void Visit(PasswordValidationException ex)
        {
            throw new NotImplementedException();
        }

        public void Visit(MailValidationException ex)
        {
            throw new NotImplementedException();
        }

        public void Visit(ItemAlreadyInUseException ex)
        {
            throw new NotImplementedException();
        }

        public void Visit(JsonDeserializeException ex)
        {
            throw new NotImplementedException();
        }

        public void Visit(JsonValidationException ex)
        {
            throw new NotImplementedException();
        }

        public void Visit(BuilderValidationException ex)
        {
            throw new NotImplementedException();
        }

        public void Visit(BuilderProccessException ex)
        {
            throw new NotImplementedException();
        }

        public void Visit(DomainValidationException ex)
        {
            throw new NotImplementedException();
        }

        public void Visit(NotFoundException ex)
        {
            throw new NotImplementedException();
        }

        public void Visit(ServiceRouteException ex)
        {
            throw new NotImplementedException();
        }
    }
}