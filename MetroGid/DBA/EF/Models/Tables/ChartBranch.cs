namespace MetroGid.DBA.EF.Models.Tables;

public partial class ChartBranch
{
    public int Id { get; set; }

    public int ChartId { get; set; }

    public int BranchId { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual Chart Chart { get; set; } = null!;
}
