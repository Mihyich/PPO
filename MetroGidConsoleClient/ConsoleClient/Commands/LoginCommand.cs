using System.ComponentModel;
using ConsoleClient.Services.Interfaces;
using ConsoleClient.SharedDTO.Auth;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ConsoleClient.Commands.Concrete;

public class LoginCommand(
    IApiService apiService
) : AsyncCommand<LoginCommand.Settings>
{
    private readonly IApiService _apiService = apiService;

    public class Settings : CommandSettings
    {
        [CommandOption("--login|-l")]
        [Description("Логин пользователя")]
        public string? Login { get; set; }

        [CommandOption("--password|-p")]
        [Description("Пароль пользователя")]
        public string? Password { get; set; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(Login))
                return ValidationResult.Error("Требуется --login|-l");

            if (string.IsNullOrWhiteSpace(Password))
                return ValidationResult.Error("Требуется --password|-p");

            return ValidationResult.Success();
        }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        AuthResponseDTO? result = await _apiService.LogInAsync(settings.Login!, settings.Password!);
        
        if (result != null)
        {
            AnsiConsole.MarkupLine("[green] Вход успешен[/]");

            var table = new Table()
                .RoundedBorder()
                .AddColumn("Поле")
                .AddColumn("Значение")
                .AddRow("ID", result.Id.ToString())
                .AddEmptyRow()
                .AddRow("Логин", result.Login)
                .AddEmptyRow()
                .AddRow("Роль", result.Role)
                .AddEmptyRow()
                .AddRow("Токен", result.Token);

            AnsiConsole.Write(table);
            return 0;
        }
        else
        {
            AnsiConsole.MarkupLine("[red] Ошибка входа[/]");
            return 1;
        }
    }
}