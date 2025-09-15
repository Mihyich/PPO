namespace MetroGid.Controllers.Utility.DTO.Auth;

public record UnauthorizedResponseDTO(
    string Error,
    string Message
);