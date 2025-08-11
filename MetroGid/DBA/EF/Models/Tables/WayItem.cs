using MetroGid.DBA.EF.Models.UserDefinedTypes;

namespace MetroGid.DBA.EF.Models.Tables;

public partial class WayItem
{
    public int Id { get; set; }

    public int WayId { get; set; }

    public string Nexus { get; set; } = null!;

    public int StepNomer { get; set; }

    public virtual Way Way { get; set; } = null!;

    public virtual WayItemRailway? WayItemRailway { get; set; }

    public virtual WayItemStation? WayItemStation { get; set; }

    public virtual WayItemTransition? WayItemTransition { get; set; }
}
