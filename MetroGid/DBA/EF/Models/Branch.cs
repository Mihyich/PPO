namespace MetroGid.DBA.EF.Models;

public partial class Branch
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int? Color { get; set; }

    public virtual ICollection<BranchStation> BranchStations { get; set; } = new List<BranchStation>();

    public virtual ChartBranch? ChartBranch { get; set; }
}
