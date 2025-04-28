using MetroGid.Core.Models.Types;

namespace MetroGid.Core.Models.Concrete
{
    public class Client(string login, string password, string mail, RoleType role = RoleType.UNSIGNED)
    {
        public string Login { get; set; } = login;
        public string Password { get; set; } = password;
        public string Mail { get; set; } = mail;
        public RoleType Role { get; set; } = role;
    }
}