namespace MetroGid.DBA.EF.Models.Tables;

public partial class Way
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public int ChartId { get; set; }

    public string Title { get; set; } = null!;

    public TimeSpan? Duration { get; set; }

    public DateTime InitDate { get; set; }

    public virtual Chart Chart { get; set; } = null!;

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<WayItem> WayItems { get; set; } = new List<WayItem>();
}
