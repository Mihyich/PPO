using MetroGid.Core.Models.Interfaces;
using MetroGid.Core.Models.Super;
using MetroGid.Core.Models.Types;
using MetroGid.Core.Utilities.Validators.Interfaces;

namespace MetroGid.Core.Models.Concrete;

public class Station(
    string title, int occupancy, AccessType type,
    TimeOnly opentime, TimeOnly closetime) :
    TemporaryAvailability(opentime, closetime),
    IAccessAvailability, IDomainValidatorAccepter
{
    public string Title { get; set; } = title;
    public int Occupancy { get; } = occupancy;
    public AccessType Type { get; } = type;
    public List<Transition> Transitions { get; } = [];
    public Railway? Prev { get; set; }
    public Railway? Next { get; set; }
    public Branch? Branch { get; set; }

    public bool HasPrev() => Prev != null;
    public bool HasNext() => Next != null;


    public override int GetHashCode() =>
        Branch == null ? 0 : HashCode.Combine(Title, Branch.Title);

    public bool IsAccessible() => Type == AccessType.ACCESSIBLE;

    public void Validate(IDomainValidatorVisitor visitor) => visitor.Visit(this);
}