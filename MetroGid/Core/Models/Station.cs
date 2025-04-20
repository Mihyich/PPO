using MetroGid.Core.Utilities.Validators.Interfaces;

namespace MetroGid.Core.Models
{
    public class Station(
        string title, int occupancy, AccessType type,
        TimeOnly opentime, TimeOnly closetime) : IDomainValidatorAccepter
    {
        public string Title { get; set; } = title;
        public int Occupancy { get; set; } = occupancy;
        public AccessType Type { get; set; } = type;
        public TimeOnly OpenTime { get; set; } = opentime;
        public TimeOnly CloseTime { get; set; } = closetime;
        public List<Transition> Transitions { get; set; } = [];
        public Railway? Prev { get; set; }
        public Railway? Next { get; set; }
        public Branch? Branch { get; set; }

        public bool HasPrev() => Prev != null;
        public bool HasNext() => Next != null;

        public override int GetHashCode() =>
            Title == null || Branch == null || Branch.Title == null ?
            0 :
            HashCode.Combine(Title, Branch.Title);

        public override bool Equals(object? obj) =>
            !(obj is null || GetType() != obj.GetType() ||
            Branch == null || ((Station)obj).Branch == null) &&
            Title == ((Station)obj).Title &&
            Branch.Title == ((Station)obj).Branch?.Title;

        public void Validate(IDomainValidatorVisitor visitor) => visitor.Visit(this);
    }
}