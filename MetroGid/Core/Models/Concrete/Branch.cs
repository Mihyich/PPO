using MetroGid.Core.Models.Interfaces;
using MetroGid.Core.Models.Types;
using MetroGid.Core.Utility.Validators.Interfaces;

namespace MetroGid.Core.Models.Concrete;

public class Branch(string title, int color, AccessType type) : IAccessAvailability, IDomainValidatorAccepter
{
    public string Title { get; set; } = title;
    public int Color { get; } = color;
    public AccessType Type { get; } = type;
    public List<Station> Stations { get; } = [];
    public Chart? Chart { get; set; }

    public bool IsAccessible() => Type == AccessType.ACCESSIBLE;

    public void Validate(IDomainValidatorVisitor visitor) => visitor.Visit(this);
}