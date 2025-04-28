namespace MetroGid.Core.Exceptions.Classification
{
    public enum ExceptionReason
    {
        ValidationFailed,
        ItemAlreadyInUse,
        FailedJsonDeserializing,
        IncorrectJsonFormat,
        IncorrectLink,
        NullResult,
        NullArgument,
        EmptyString,
        StringLenghtOutOfRange,
        ValueOutOfRange,
        ValueDuplicate,
        NotLogicValue,
        UnexpectedBehavior,
        NotFound
    }
}