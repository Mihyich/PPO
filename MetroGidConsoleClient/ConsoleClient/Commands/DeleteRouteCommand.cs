using System.ComponentModel;
using System.Text.Json;
using ConsoleClient.Services.Interfaces;
using ConsoleClient.SharedDTO.Concrete;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ConsoleClient.Commands.Concrete;

public class DeleteRouteCommand(
    IApiService apiService
) : AsyncCommand<DeleteRouteCommand.Settings>
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

        [CommandOption("--title|-t")]
        [Description("Название маршрута")]
        public string? RouteTilte { get; set; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(City))
                return ValidationResult.Error("Требуется --city|-c");

            if (string.IsNullOrWhiteSpace(Chart))
                return ValidationResult.Error("Требуется --chart");

            if (string.IsNullOrWhiteSpace(RouteTilte))
                return ValidationResult.Error("Требуется --title|-t");

            return ValidationResult.Success();
        }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        return await _apiService.DeleteRoute(
            settings.City!, settings.Chart!, settings.RouteTilte!
        );
    }
}