namespace MetroGid.DBA.EF.Models;

public partial class Chart
{
    public int Id { get; set; }

    public string City { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? SvgContent { get; set; }

    public virtual ICollection<ChartBranch> ChartBranches { get; set; } = new List<ChartBranch>();

    public virtual ICollection<Way> Ways { get; set; } = new List<Way>();
}
