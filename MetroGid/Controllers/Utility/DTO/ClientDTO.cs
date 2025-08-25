namespace MetroGid.Controllers.Utility.DTO;

public record ClientDTO(
    string Login,
    string Password,
    string Mail,
    RoleTypeDTO Role
);