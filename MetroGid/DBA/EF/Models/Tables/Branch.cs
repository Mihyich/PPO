using MetroGid.DBA.EF.Models.UserDefinedTypes;

namespace MetroGid.DBA.EF.Models.Tables;

public partial class Branch
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int? Color { get; set; }

    public AccessType Access { get; set; } = AccessType.ACCESSIBLE;

    public virtual ICollection<BranchStation> BranchStations { get; set; } = [];

    public virtual ChartBranch? ChartBranch { get; set; }
}
