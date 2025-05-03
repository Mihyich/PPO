using MetroGid.Core.Models.Types;
using MetroGid.Core.Utilities.Validators.Interfaces;

namespace MetroGid.Core.Models.Concrete;

public class Client(
    string login, string password, string mail,
    RoleType role = RoleType.UNSIGNED
) : IDomainValidatorAccepter
{
    public string Login { get; set; } = login;
    public string Password { get; set; } = password;
    public string Mail { get; set; } = mail;
    public RoleType Role { get; set; } = role;

    public void Validate(IDomainValidatorVisitor visitor) => visitor.Visit(this);
}