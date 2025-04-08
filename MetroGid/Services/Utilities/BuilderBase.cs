namespace MetroGid.Services.Utilities
{
    public abstract class BuilderBase<T>
    {
        protected T? Chart;

        public abstract T? GetResult();
    }
}