using MetroGid.Core.Exceptions.Interfaces;

namespace MetroGid.Core.Exceptions.Super;

public abstract class SuperExceptionHandler
{
    public T? Snap<T>(Func<T> func, IExceptionVisitor? logger = null)
    {
        try
        {
            return func();
        }
        catch (SuperException ex) when (ShouldHandle(ex))
        {
            if (logger != null)
                ex.Accept(logger);


            return HandleException<T>(ex);
        }
    }

    public void Snap(Action action, IExceptionVisitor? logger = null)
    {
        try
        {
            action();
        }
        catch (SuperException ex) when (ShouldHandle(ex))
        {
            if (logger != null)
                ex.Accept(logger);

            HandleException(ex);
        }
    }

    protected abstract bool ShouldHandle(SuperException ex);

    protected abstract T? HandleException<T>(SuperException ex);

    protected abstract void HandleException(SuperException ex);
}