using MetroGid.Core.Utilities.Validators.Interfaces;

namespace MetroGid.Core.Models
{
    public class Transition(
        int occupancy, AccessType type, TimeOnly duration,
        TimeOnly opentime, TimeOnly closetime) : TemporaryAvailability(type, opentime, closetime), IDomainValidatorAccepter
    {
        public int Occupancy { get; set; } = occupancy;
        public TimeOnly Duration { get; set; } = duration;
        public Station? From { get; set; }
        public Station? To { get; set; }

        public bool InConnect(Station station) =>
            From != null && To != null &&
            (station == From || station == To);

        public Station? ToFrom(Station cur) =>
            cur.Equals(From) ? To :
            cur.Equals(To) ? From : null;

        public void Validate(IDomainValidatorVisitor visitor) => visitor.Visit(this);
    }
}