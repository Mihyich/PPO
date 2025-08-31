namespace ConsoleClient.SharedDTO.Auth;

public record LoginRequestDTO(
    string Login,
    string Password
);