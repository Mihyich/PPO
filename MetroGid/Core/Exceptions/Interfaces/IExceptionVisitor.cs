using MetroGid.Core.Exceptions.Concrete;

namespace MetroGid.Core.Exceptions.Interfaces;

public interface IExceptionVisitor
{
    void Visit(JsonDeserializeException ex);
    void Visit(JsonValidationException ex);
    void Visit(BuilderValidationException ex);
    void Visit(BuilderProccessException ex);
    void Visit(DomainValidationException ex);
    void Visit(ServiceRouteException ex);
    void Visit(DataBaseException ex);
}