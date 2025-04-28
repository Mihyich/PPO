using MetroGid.Core.Models.Interfaces;
using MetroGid.Core.Models.Super;
using MetroGid.Core.Models.Types;
using MetroGid.Core.Utilities.Validators.Interfaces;

namespace MetroGid.Core.Models.Concrete
{
    public class Transition(
        int occupancy, AccessType type, TimeOnly duration,
        TimeOnly opentime, TimeOnly closetime) : TemporaryAvailability(opentime, closetime), IAccessAvailability, IDomainValidatorAccepter
    {
        public int Occupancy { get; set; } = occupancy;
        public AccessType Type { get; set; } = type;
        public TimeOnly Duration { get; set; } = duration;
        public Station? From { get; set; }
        public Station? To { get; set; }

        public bool InConnect(Station station) =>
            From != null && To != null &&
            (station == From || station == To);

        public Station? ToFrom(Station cur) =>
            cur.Equals(From) ? To :
            cur.Equals(To) ? From : null;

        public bool IsAccessible() => Type == AccessType.ACCESSIBLE;

        public void Validate(IDomainValidatorVisitor visitor) => visitor.Visit(this);
    }
}