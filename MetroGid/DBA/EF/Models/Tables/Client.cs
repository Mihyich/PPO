using MetroGid.DBA.EF.Models.UserDefinedTypes;

namespace MetroGid.DBA.EF.Models.Tables;

public partial class Client
{
    public int Id { get; set; }

    public string ClientLogin { get; set; } = null!;

    public string ClientPassword { get; set; } = null!;

    public string Mail { get; set; } = null!;

    public RoleType Privilege { get; set; } = RoleType.UNSIGNED;

    public virtual ICollection<Station> Stations { get; set; } = [];

    public virtual ICollection<Transition> Transitions { get; set; } = [];

    public virtual ICollection<Way> Ways { get; set; } = [];
}
