using MetroGid.Core.Exceptions.Classification;
using MetroGid.Core.Exceptions.Concrete;
using MetroGid.Core.Models;

namespace MetroGid.Core.Utilities
{
    public class BuilderChart : BuilderChartBase
    {
        protected List<Branch> Branches = [];

        public override void BuildBranch(string title, int color, AccessType type)
        {
            Branch branch = new(title, color, type);
            Branches.Add(branch);
        }

        public override void BuildStation(
            string branch_title,
            string title, int occupancy, AccessType type, TimeOnly opentime, TimeOnly closetime
        )
        {
            Branch? branch;

            if ((branch = Branches.FirstOrDefault(b => b.Title == branch_title)) != null)
            {
                Station station = new(title, occupancy, type, opentime, closetime);
                branch.Stations.Add(station);
                station.Branch = branch;
            }
        }

        public override void BuildRailway(
            string branch_title, string station_title_src, string station_title_dst,
            TimeOnly duration
        )
        {
            Branch? branch;
            Station? src;
            Station? dst;

            if (
                station_title_src != station_title_dst &&
                (branch = Branches.FirstOrDefault(b => b.Title == branch_title)) != null &&
                (src = branch.Stations.FirstOrDefault(s => s.Title == station_title_src)) != null &&
                (dst = branch.Stations.FirstOrDefault(s => s.Title == station_title_dst)) != null
            )
            {
                Railway railway = new(duration);

                if (!src.HasNext() && !dst.HasPrev())
                {
                    railway.Prev = src;
                    railway.Next = dst;

                    src.Next = railway;
                    dst.Prev = railway;
                }
                else if (!dst.HasNext() && !src.HasPrev())
                {
                    railway.Prev = dst;
                    railway.Next = src;

                    dst.Next = railway;
                    src.Prev = railway;
                }
            }
        }

        public override void BuildTransition(
            string branch_title_src, string station_title_src, string branch_title_dst, string station_title_dst,
            int occupancy, AccessType type, TimeOnly duration, TimeOnly opentime, TimeOnly closetime
        )
        {
            Branch? branch_src;
            Branch? branch_dst;
            Station? station_src;
            Station? station_dst;

            if (
                branch_title_src != branch_title_dst &&
                (branch_src = Branches.FirstOrDefault(b => b.Title == branch_title_src)) != null &&
                (branch_dst = Branches.FirstOrDefault(b => b.Title == branch_title_dst)) != null &&
                (station_src = branch_src.Stations.FirstOrDefault(s => s.Title == station_title_src)) != null &&
                (station_dst = branch_dst.Stations.FirstOrDefault(s => s.Title == station_title_dst)) != null
            )
            {
                Transition transition = new(occupancy, type, duration, opentime, closetime)
                {
                    From = station_src,
                    To = station_dst
                };

                station_src.Transitions.Add(transition);
                station_dst.Transitions.Add(transition);
            }
        }

        public override void BuildChart(string title, string city, string svg_inst)
        {
            Chart = new(title, city, svg_inst)
            {
                Branches = Branches
            };
        }

        public override Chart GetResult() =>
            Chart ??
                throw new BuilderValidationException(
                    "BuilderChart не создал конечный продукт (null)",
                    ExceptionType.Error,
                    ExceptionReason.NullResult);
    }
}