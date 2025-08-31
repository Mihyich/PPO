using System.ComponentModel;
using System.Text.Json;
using ConsoleClient.Services.Interfaces;
using ConsoleClient.SharedDTO.Concrete;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ConsoleClient.Commands.Concrete;

public class GetSavedRouteCommand(
    IApiService apiService
) : AsyncCommand<GetSavedRouteCommand.Settings>
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
        RouteDTO? route = await _apiService.GetSavedRoute(
            settings.City!, settings.Chart!, settings.RouteTilte!
        );

        if (route != null)
        {
            string filePath = "temp_get_saved_route.json";

            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(route, options));

                Console.WriteLine($"Маршрут сохранён в файл: {Path.GetFullPath(filePath)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
                return -1;
            }
        }

        return 0;
    }
}