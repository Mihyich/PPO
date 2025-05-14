namespace MetroGid.DBA.EF.Models;

public partial class Station
{
    public int Id { get; set; }

    public int? DutyId { get; set; }

    public string Title { get; set; } = null!;

    public short? Occupancy { get; set; }

    public AccessType Access { get; set; } = AccessType.ACCESSIBLE;

    public TimeOnly OpenTime { get; set; }

    public TimeOnly CloseTime { get; set; }

    public virtual BranchStation? BranchStation { get; set; }

    public virtual Client? Duty { get; set; }

    public virtual ICollection<Railway> RailwayFroms { get; set; } = [];

    public virtual ICollection<Railway> RailwayTos { get; set; } = [];

    public virtual ICollection<StationTransition> StationTransitions { get; set; } = [];

    public virtual ICollection<WayItemStation> WayItemStations { get; set; } = [];
}
