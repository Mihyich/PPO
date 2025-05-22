namespace MetroGid.Core.Utilities.Builders;

public abstract class BuilderBase<T>
{
    protected T? Result;

    public abstract T GetResult();
}