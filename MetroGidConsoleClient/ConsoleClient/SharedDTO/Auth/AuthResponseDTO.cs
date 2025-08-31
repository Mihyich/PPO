namespace ConsoleClient.SharedDTO.Auth;

public record AuthResponseDTO(
    int Id,
    string Token,
    string Login,
    string Role
);