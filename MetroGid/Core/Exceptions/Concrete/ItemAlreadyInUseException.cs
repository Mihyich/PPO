using System.Runtime.CompilerServices;
using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Exceptions.Super;

namespace MetroGid.Core.Exceptions.Concrete
{
    public class ItemAlreadyInUseException(
        string message,
        ExceptionType extType,
        ExceptionReason excReason,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        Exception? innerException = null
    ) : SuperException(
        message, extType, excReason,
        filePath, lineNumber, innerException)
    {
        public override void Accept(IExceptionVisitor visitor) => visitor.Visit(this);
    }
}