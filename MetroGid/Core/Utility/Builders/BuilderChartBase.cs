using MetroGid.Core.Models.Concrete;
using MetroGid.Core.Models.Types;
using MetroGid.Core.Utility.Validators.Interfaces;

namespace MetroGid.Core.Utility.Builders;

public abstract class BuilderChartBase(
    IDomainValidatorVisitor domainAttribsValidator,
    IDomainValidatorVisitor domainReferentialityValidator
)
{
    protected IDomainValidatorVisitor DomainAttribsValidator = domainAttribsValidator;
    protected IDomainValidatorVisitor DomainReferentialityValidator = domainReferentialityValidator;

    protected Chart? Result;

    public abstract Chart GetResult();

    public abstract void BuildBranch(string title, int color, AccessType type);

    public abstract void BuildStation(
        string branch_title,
        string title, int occupancy, AccessType type, TimeOnly opentime, TimeOnly closetime
    );

    public abstract void BuildRailway(
        string branch_title, string station_title_src, string station_title_dst,
        TimeSpan duration
    );

    public abstract void BuildTransition(
        string branch_title_src, string station_title_src, string branch_title_dst, string station_title_dst,
        int occupancy, AccessType type, TimeSpan duration, TimeOnly opentime, TimeOnly closetime
    );

    public abstract void BuildChart(string title, string city, string svg_inst);
}