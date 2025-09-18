namespace ConsoleClient.SharedDTO;

public record ErrorResponse
(
    string? Error,
    string? Message
);