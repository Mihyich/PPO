namespace MetroGid.DBA.EF.Models.Tables;

public partial class StationTransition
{
    public int Id { get; set; }

    public int StationId { get; set; }

    public int TransitionId { get; set; }

    public virtual Station Station { get; set; } = null!;

    public virtual Transition Transition { get; set; } = null!;
}
