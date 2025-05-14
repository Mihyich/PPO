namespace MetroGid.DBA.EF.Models;

public partial class WayItemStation
{
    public int Id { get; set; }

    public int WayItemId { get; set; }

    public int StationId { get; set; }

    public virtual Station Station { get; set; } = null!;

    public virtual WayItem WayItem { get; set; } = null!;
}
