using MetroGid.Core.Utilities.Strategies;
using MetroGid.Core.Utilities.Validators.Interfaces;

namespace MetroGid.Core.Models.Concrete;

public class Chart(string title, string city, string svg_inst) : IDomainValidatorAccepter
{
    public string Title { get; set; } = title;
    public string City { get; set; } = city;
    public string SvgInst { get; set; } = svg_inst;
    public List<Branch> Branches { get; set; } = [];

    public Station? GetStation(string branchTitle, string stationTitle) =>
        Branches
            .FirstOrDefault(b => b.Title == branchTitle)?.Stations?
                .FirstOrDefault(s => s.Title == stationTitle);

    public Route Search(Station src, Station dst, TimeOnly timeStart, StrategySearchRouteBase Searcher)
    {
        Route route = Searcher.Search(Branches, src, dst, timeStart);
        route.Title = "Новый маршрут";
        route.Chart = this;

        return route;
    }

    public void Validate(IDomainValidatorVisitor visitor) => visitor.Visit(this);
}