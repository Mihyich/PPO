using MetroGid.Core.Models;

namespace MetroGid.Core.Utilities
{
    public abstract class DirectorChartBase(BuilderChartBase builder) : DirectorBase<Chart>
    {
        protected BuilderChartBase Builder = builder;
    }
}