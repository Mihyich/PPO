namespace MetroGid.Controllers.Utility.DTO.Auth;

public record TokenClientDTO(
    int Id,
    string Login,
    string Role
);