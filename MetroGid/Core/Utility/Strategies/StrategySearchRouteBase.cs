using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utility.TimeMeter.Super;

namespace MetroGid.Core.Utility.Strategies;

public abstract class StrategySearchRouteBase(TimeSuper timerSuper)
{
    protected readonly TimeSuper ts = timerSuper;

    public abstract Route? Search(Chart chart, Station src, Station dst, TimeOnly timeStart);
}