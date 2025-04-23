namespace MetroGid.Core.Utilities.Builders
{
    public abstract class BuilderBase<T>
    {
        protected T? Chart;

        public abstract T GetResult();
    }
}