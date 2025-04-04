namespace MetroGid.Services.Utilities
{
    public abstract class DirectorBase<T>(BuilderBase<T> Builder)
    {
        protected BuilderBase<T> Builder = Builder;

        public abstract T Construct();
    }
}