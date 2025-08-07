using MetroGid.Core.Exceptions.Super;

namespace MetroGid.Core.Exceptions.Handlers;

public class PassThroughHandlerException : SuperExceptionHandler
{
    protected override bool ShouldHandle(SuperException ex) => false;

    protected override T? HandleException<T>(SuperException ex) where T : default => default;

    protected override void HandleException(SuperException ex) { return; }
}