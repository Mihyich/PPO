namespace MetroGid.Core.Utility.Validators.Interfaces;

public interface IDomainValidatorAccepter
{
    void Validate(IDomainValidatorVisitor visitor);
}