using MetroGid.Core.Utilities.Validators.Interfaces;

namespace MetroGid.Core.Models
{
    public class Railway(TimeOnly duration) : IDomainValidatorAccepter
    {
        public Station? Prev { get; set; }
        public Station? Next { get; set; }
        public TimeOnly Duration { get; set; } = duration;

        public void Validate(IDomainValidatorVisitor visitor) => visitor.Visit(this);
    }
}