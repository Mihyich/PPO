namespace MetroGid.Services.Models
{
    public class Client
    {
        public string Login {get; set;} = string.Empty;
        public string Password {get; set;} = string.Empty;
        public string Mail {get; set;} = string.Empty;
        public RoleType Role {get; set;} = RoleType.UNSIGNED;
    }
}