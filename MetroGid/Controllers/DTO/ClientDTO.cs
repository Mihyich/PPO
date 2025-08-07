namespace MetroGid.Controllers.DTO;

public record ClientDTO(
    string Login,
    string Password,
    string Mail,
    RoleTypeDTO Role
);