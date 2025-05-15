namespace MetroGid.DBA.EF.Models.Tables;

public partial class Railway
{
    public int Id { get; set; }

    public int FromId { get; set; }

    public int ToId { get; set; }

    public TimeOnly Duration { get; set; }

    public virtual Station From { get; set; } = null!;

    public virtual Station To { get; set; } = null!;

    public virtual ICollection<WayItemRailway> WayItemRailways { get; set; } = [];
}
