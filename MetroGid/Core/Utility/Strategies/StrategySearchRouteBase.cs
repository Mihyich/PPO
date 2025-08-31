using MCMC = MetroGid.Core.Models.Concrete;
using MetroGid.Core.Utility.TimeMeter.Super;

namespace MetroGid.Core.Utility.Strategies;

public abstract class StrategySearchRouteBase(TimeSuper timerSuper)
{
    protected readonly TimeSuper ts = timerSuper;

    public abstract MCMC.Route? Search(MCMC.Chart chart, MCMC.Station src, MCMC.Station dst, TimeOnly timeStart);
}