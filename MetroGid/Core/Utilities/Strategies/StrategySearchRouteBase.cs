using MetroGid.Core.Models.Concrete;

namespace MetroGid.Core.Utilities.Strategies;

public abstract class StrategySearchRouteBase
{
    public abstract Route Search(Chart chart, Station src, Station dst, TimeOnly timeStart);
}