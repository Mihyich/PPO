using MetroGid.Core.Models;

namespace MetroGid.Core.Utilities
{
    public abstract class BuilderChartBase : BuilderBase<Chart>
    {

        public abstract void BuildBranch(string title, int color, AccessType type);

        public abstract void BuildStation(
            string branch_title,
            string title, int occupancy, AccessType type, TimeOnly opentime, TimeOnly closetime
        );

        public abstract void BuildRailway(
            string branch_title, string station_title_src, string station_title_dst,
            TimeOnly duration
        );

        public abstract void BuildTransition(
            string branch_title_src, string station_title_src, string branch_title_dst, string station_title_dst,
            int occupancy, AccessType type, TimeOnly duration, TimeOnly opentime, TimeOnly closetime
        );

        public abstract void BuildChart(string title, string city, string svg_inst);
    }
}