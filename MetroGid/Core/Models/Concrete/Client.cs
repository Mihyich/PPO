using MetroGid.Core.Models.Types;
using MetroGid.Core.Utility.Validators.Interfaces;

namespace MetroGid.Core.Models.Concrete;

public class Client(
    string login, string password, string mail,
    RoleType role = RoleType.UNSIGNED
) : IDomainValidatorAccepter
{
    public string Login { get; } = login;
    public string Password { get; } = password;
    public string Mail { get; } = mail;
    public RoleType Role { get; } = role;

    public void Validate(IDomainValidatorVisitor visitor) => visitor.Visit(this);
}