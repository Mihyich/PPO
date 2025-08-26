namespace MetroGid.Controllers.Utility.DTO.Auth;

public record AuthResponseDTO(
    int Id,
    string Token,
    string Login,
    string Role
);