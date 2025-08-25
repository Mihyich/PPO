using MetroGid.Core.Utility.Strategies;
using MetroGid.Core.Utility.Validators.Interfaces;

namespace MetroGid.Core.Models.Concrete;

public class Chart(string title, string city, string svg_inst) : IDomainValidatorAccepter
{
    public string Title { get; } = title;
    public string City { get; } = city;
    public string SvgInst { get; } = svg_inst;
    public List<Branch> Branches { get; set; } = [];

    public Station? GetStation(string branchTitle, string stationTitle) =>
        Branches
            .FirstOrDefault(b => b.Title == branchTitle)?.Stations?
                .FirstOrDefault(s => s.Title == stationTitle);

    public Route? Search(Station src, Station dst, TimeOnly timeStart, StrategySearchRouteBase searcher)
    {
        Route? route = searcher.Search(this, src, dst, timeStart);

        if (route != null)
        {
            route.Chart = this;
            route.Title = $"[\"{City}\":\"{Title}\"]:[\"{src?.Branch?.Title}\":\"{src?.Title}\"]:[\"{dst?.Branch?.Title}\":\"{dst?.Title}\"]";
        }

        return route;
    }

    public void Validate(IDomainValidatorVisitor visitor) => visitor.Visit(this);
}