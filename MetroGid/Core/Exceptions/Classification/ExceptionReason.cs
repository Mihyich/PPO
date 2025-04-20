namespace MetroGid.Core.Exceptions.Classification
{
    public enum ExceptionReason
    {
        ValidationFailed,
        ItemAlreadyInUse,
        FailedJsonDeserializing,
        IncorrectJsonFormat,
        NullResult,
        NullArgument,
        NotFound,
        AccessDenied,
    }
}