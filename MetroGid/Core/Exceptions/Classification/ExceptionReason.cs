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
        EmptyString,
        StringLenghtOutOfRange,
        ValueOutOfRange,
        NotLogicValue,
        UnexpectedBehavior,
        NotFound
    }
}