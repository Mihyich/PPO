namespace MetroGid.Controllers.Utility.DTO.Auth;

public record RegisterRequestDTO(
    string Login,
    string Password,
    string Mail
);