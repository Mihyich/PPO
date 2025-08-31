using System.ComponentModel;
using ConsoleClient.Services.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ConsoleClient.Commands.Concrete;

public class SaveRouteCommand(
    IApiService apiService
) : AsyncCommand<SaveRouteCommand.Settings>
{
    private readonly IApiService _apiService = apiService;

    public class Settings : CommandSettings
    {
        [CommandOption("--route|-r")]
        [Description("Путь к Json маршруту")]
        public string? Route { get; set; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(Route))
                return ValidationResult.Error("Требуется --route|-r");

            return ValidationResult.Success();
        }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        if (!File.Exists(settings.Route!))
        {
            Console.WriteLine("Файл не найден.");
            return 1;
        }

        string json = "";

        try
        {
            json = await File.ReadAllTextAsync(settings.Route!);
            json = json.Replace("\\\"", "");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            return 1;
        }

        return await _apiService.SaveRoute(json);
    }
}