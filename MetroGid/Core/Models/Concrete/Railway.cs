using MetroGid.Core.Utility.Validators.Interfaces;

namespace MetroGid.Core.Models.Concrete;

public class Railway(TimeOnly duration) : IDomainValidatorAccepter
{
    public Station? Prev { get; set; }
    public Station? Next { get; set; }
    public TimeOnly Duration { get; } = duration;

    public void Validate(IDomainValidatorVisitor visitor) => visitor.Visit(this);
}