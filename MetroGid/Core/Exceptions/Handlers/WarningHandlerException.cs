using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Super;

namespace MetroGid.Core.Exceptions.Handlers
{
    public class WarningHandlerException : SuperHandlerException
    {
        protected override bool ShouldHandle(SuperException ex) =>
            ex.ExcType == ExceptionType.Warning ||
            ex.ExcType == ExceptionType.Quiet;

        protected override T? HandleException<T>(SuperException ex) where T : default => default;
    }
}