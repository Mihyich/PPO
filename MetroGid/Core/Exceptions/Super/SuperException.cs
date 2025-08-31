using System.Runtime.CompilerServices;
using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Interfaces;

namespace MetroGid.Core.Exceptions.Super;

public abstract class SuperException(
    string message,
    ExceptionType excType,
    ExceptionReason excReason,
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = 0,
    Exception? innerException = null) : Exception(message, innerException), IExceptionAccepter
{
    public DateTime OccurAt { get; } = DateTime.UtcNow;
    public string FilePath { get; } = filePath;
    public int LineNumber { get; } = lineNumber;
    public readonly ExceptionType ExcType = excType;
    public readonly ExceptionReason ExcReason = excReason;

    public string GetInfo() =>
        $"[{ExcType}]:[{OccurAt}]:({Path.GetFileName(FilePath)}):[{LineNumber}]:{base.Message}";

    public abstract void Accept(IExceptionVisitor visitor);
}