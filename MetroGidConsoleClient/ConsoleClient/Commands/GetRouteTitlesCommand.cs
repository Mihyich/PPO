using System.ComponentModel;
using ConsoleClient.Services.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ConsoleClient.Commands.Concrete;

public class GetRouteTitlesCommand(
    IApiService apiService
) : AsyncCommand<GetRouteTitlesCommand.Settings>
{
    private readonly IApiService _apiService = apiService;

    public class Settings : CommandSettings
    {
        [CommandOption("--city|-c")]
        [Description("Город")]
        public string? City { get; set; }

        [CommandOption("--chart")]
        [Description("Схема")]
        public string? Chart { get; set; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(City))
                return ValidationResult.Error("Требуется --city|-c");

            if (string.IsNullOrWhiteSpace(Chart))
                return ValidationResult.Error("Требуется --chart");

            return ValidationResult.Success();
        }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        List<string> titles = await _apiService.GetRouteTitles(
            settings.City!,
            settings.Chart!
        );

        AnsiConsole.WriteLine($"Названия маршрутов для города {settings.City!} схемы {settings.Chart!}:");

        foreach (string title in titles)
            Console.WriteLine(title);

        return 0;
    }
}