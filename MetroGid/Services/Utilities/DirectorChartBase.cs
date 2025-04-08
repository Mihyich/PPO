using MetroGid.Services.Models;

namespace MetroGid.Services.Utilities
{
    public abstract class DirectorChartBase(BuilderChartBase builder) : DirectorBase<Chart>
    {
        protected BuilderChartBase Builder = builder;
    }
}