using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utilities.TimeMeter.Super;

namespace MetroGid.Core.Utilities.Strategies;

public abstract class StrategySearchRouteBase(TimeSuper timerSuper)
{
    protected readonly TimeSuper ts = timerSuper;

    public abstract Route? Search(Chart chart, Station src, Station dst, TimeOnly timeStart);
}