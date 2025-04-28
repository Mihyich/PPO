using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Models.Types;
using MetroGid.Core.Utilities.Validators.Interfaces;

namespace MetroGid.Core.Utilities.Builders
{
    public abstract class BuilderChartBase(
        IDomainValidatorVisitor domainAttribsValidator,
        IDomainValidatorVisitor domainReferentialityValidator
    ) : BuilderBase<Chart>
    {
        protected IDomainValidatorVisitor DomainAttribsValidator = domainAttribsValidator;
        protected IDomainValidatorVisitor DomainReferentialityValidator = domainReferentialityValidator;

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