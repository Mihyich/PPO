namespace MetroGid.DBA.EF.Models;

public partial class WayItemTransition
{
    public int Id { get; set; }

    public int WayItemId { get; set; }

    public int TransitionId { get; set; }

    public virtual Transition Transition { get; set; } = null!;

    public virtual WayItem WayItem { get; set; } = null!;
}
