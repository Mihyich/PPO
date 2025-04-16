namespace MetroGid.Core.Utilities
{
    public abstract class BuilderBase<T>
    {
        protected T? Chart;

        public abstract T? GetResult();
    }
}