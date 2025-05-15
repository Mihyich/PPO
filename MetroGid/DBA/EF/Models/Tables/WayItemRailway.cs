namespace MetroGid.DBA.EF.Models.Tables;

public partial class WayItemRailway
{
    public int Id { get; set; }

    public int WayItemId { get; set; }

    public int RailwayId { get; set; }

    public virtual Railway Railway { get; set; } = null!;

    public virtual WayItem WayItem { get; set; } = null!;
}
