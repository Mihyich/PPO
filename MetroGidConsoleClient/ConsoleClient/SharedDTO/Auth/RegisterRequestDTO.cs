namespace ConsoleClient.SharedDTO.Auth;

public record RegisterRequestDTO(
    string Login,
    string Password,
    string Mail
);