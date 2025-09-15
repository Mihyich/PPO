namespace MetroGid.Controllers.Utility.DTO.Concrete;

public record ClientDTO(
    string Login,
    string Password,
    string Mail,
    RoleTypeDTO Role
);