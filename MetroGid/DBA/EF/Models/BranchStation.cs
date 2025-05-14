namespace MetroGid.DBA.EF.Models;

public partial class BranchStation
{
    public int Id { get; set; }

    public int BranchId { get; set; }

    public int StationId { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual Station Station { get; set; } = null!;
}
