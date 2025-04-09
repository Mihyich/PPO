using MetroGid.Services.Models;

namespace MetroGid.Services.Utilities
{
    public abstract class StrategySearchRouteBase
    {
        public abstract Route? Search(List<Branch> branches, Station src, Station dst);
    }
}