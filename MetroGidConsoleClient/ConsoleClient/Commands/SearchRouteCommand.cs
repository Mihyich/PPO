using System.ComponentModel;
using System.Text.Json;
using ConsoleClient.Services.Interfaces;
using ConsoleClient.SharedDTO.Concrete;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ConsoleClient.Commands.Concrete;

public class SearchRouteCommand(
    IApiService apiService
) : AsyncCommand<SearchRouteCommand.Settings>
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

        [CommandOption("--from-branch")]
        [Description("Ветка <С>")]
        public string? FromBranch { get; set; }

        [CommandOption("--from-station")]
        [Description("Станция <С>")]
        public string? FromStation { get; set; }

        [CommandOption("--to-branch")]
        [Description("Ветка <На>")]
        public string? ToBranch { get; set; }

        [CommandOption("--to-station")]
        [Description("Станция <На>")]
        public string? ToStation { get; set; }

        [CommandOption("--time|-t")]
        [Description("Текущее время")]
        public string? Time { get; set; }


        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(City))
                return ValidationResult.Error("Требуется --city|-c");

            if (string.IsNullOrWhiteSpace(Chart))
                return ValidationResult.Error("Требуется --chart");

            if (string.IsNullOrWhiteSpace(FromBranch))
                return ValidationResult.Error("Требуется --from-branch");

            if (string.IsNullOrWhiteSpace(FromStation))
                return ValidationResult.Error("Требуется --from-station");

            if (string.IsNullOrWhiteSpace(ToBranch))
                return ValidationResult.Error("Требуется --to-branch");

            if (string.IsNullOrWhiteSpace(ToStation))
                return ValidationResult.Error("Требуется --to-station");

            if (string.IsNullOrWhiteSpace(Time))
                return ValidationResult.Error("Требуется --time|-t");

            return ValidationResult.Success();
        }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        RouteDTO? route = await _apiService.SearchRoute(
            settings.City!, settings.Chart!,
            settings.FromBranch!, settings.FromStation!,
            settings.ToBranch!, settings.ToStation!,
            settings.Time!
        );

        if (route != null)
        {
            string filePath = "temp_search_route.json";

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

            return 0;
        }

        return 1;
    }
}