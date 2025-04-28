using MetroGid.Core.Models;
using MetroGid.Core.Models.Concrete;

namespace MetroGid.Core.Utilities.Strategies
{
    public abstract class StrategySearchRouteBase
    {
        public abstract Route? Search(List<Branch> branches, Station src, Station dst, TimeOnly timeStart);
    }
}