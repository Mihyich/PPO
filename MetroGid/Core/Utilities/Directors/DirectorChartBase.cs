using MetroGid.Core.Models;
using MetroGid.Core.Utilities.Builders;

namespace MetroGid.Core.Utilities.Directors
{
    public abstract class DirectorChartBase(BuilderChartBase builder) : DirectorBase<Chart>
    {
        protected BuilderChartBase Builder = builder;
    }
}