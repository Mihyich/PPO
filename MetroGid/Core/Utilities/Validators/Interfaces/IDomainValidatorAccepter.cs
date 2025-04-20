namespace MetroGid.Core.Utilities.Validators.Interfaces
{
    public interface IDomainValidatorAccepter
    {
        void Validate(IDomainValidatorVisitor visitor);
    }
}