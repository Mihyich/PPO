namespace MetroGid.Core.Exceptions.Interfaces;

public interface IExceptionAccepter
{
    void Accept(IExceptionVisitor visitor);
}