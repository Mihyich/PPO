namespace ConsoleClient.SharedDTO.Auth;

public record TokenClientDTO(
    int Id,
    string Login,
    string Role
);