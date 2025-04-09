using MetroGid.Services.Utilities;

namespace MetroGid.Services.Models
{
    public class Chart(string title, string city, string svg_inst)
    {
        public string Title { get; set; } = title;
        public string City { get; set; } = city;
        public string SvgInst { get; set; } = svg_inst;
        public List<Branch> Branches {get; set;} = [];

        public StrategySearchRouteBase? Searcher;

        public Station? GetStation(string branch_title, string station_title)
        {
            Branch? branch = null;
            Station? station = null;

            if ((branch = Branches.FirstOrDefault(b => b.Title == branch_title)) != null)
                station = branch.Stations.FirstOrDefault(s => s.Title == station_title);

            return station;
        }

        public Route? Search(Station src, Station dst) => Searcher?.Search(Branches, src, dst) ?? null;
    }
}