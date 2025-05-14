namespace MetroGid.DBA.EF.Models;

public partial class Transition
{
    public int Id { get; set; }

    public int? DutyId { get; set; }

    public short? Occupancy { get; set; }

    public TimeOnly Duration { get; set; }

    public TimeOnly OpenTime { get; set; }

    public TimeOnly CloseTime { get; set; }

    public virtual Client? Duty { get; set; }

    public virtual ICollection<StationTransition> StationTransitions { get; set; } = [];

    public virtual ICollection<WayItemTransition> WayItemTransitions { get; set; } = [];
}
