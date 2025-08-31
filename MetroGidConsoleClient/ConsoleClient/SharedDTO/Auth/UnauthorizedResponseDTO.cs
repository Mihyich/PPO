namespace ConsoleClient.SharedDTO.Auth;

public record UnauthorizedResponseDTO(
    string Error,
    string Message
);