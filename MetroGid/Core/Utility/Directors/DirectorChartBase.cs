using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utility.Builders;

namespace MetroGid.Core.Utility.Directors;

public abstract class DirectorChartBase(BuilderChartBase builder) : DirectorBase<Chart>
{
    protected BuilderChartBase Builder = builder;
}