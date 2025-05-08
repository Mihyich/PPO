namespace MetroGid.Controllers.DTO;

public class ClientDTO(string title, string city, string svg_inst, RoleTypeDTO role)
{
    public string Login { get; } = title;
    public string Password { get; } = city;
    public string Mail { get; } = svg_inst;
    public RoleTypeDTO Role { get; } = role;
}