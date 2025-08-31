using System.ComponentModel;
using ConsoleClient.Services.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ConsoleClient.Commands.Concrete;

public class UnRegCommand(
    IApiService apiService
) : AsyncCommand<UnRegCommand.Settings>
{
    private readonly IApiService _apiService = apiService;

    public class Settings : CommandSettings
    {
        [CommandOption("--password|-p")]
        [Description("Пароль пользователя")]
        public string? Password { get; set; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(Password))
                return ValidationResult.Error("Требуется --password|-p");

            return ValidationResult.Success();
        }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        await _apiService.UnRegAsync(settings.Password!);
        return 0;
    }
}