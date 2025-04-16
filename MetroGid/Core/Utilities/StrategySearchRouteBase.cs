using MetroGid.Core.Models;

namespace MetroGid.Core.Utilities
{
    public abstract class StrategySearchRouteBase
    {
        public abstract Route? Search(List<Branch> branches, Station src, Station dst);
    }
}