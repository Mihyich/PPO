using System.ComponentModel;
using System.Text;
using ConsoleClient.Services.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ConsoleClient.Commands.Concrete;

public class GetChartSchemeCommand(
    IApiService apiService
) : AsyncCommand<GetChartSchemeCommand.Settings>
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
                return ValidationResult.Error("Требуется --chart|-ch");

            return ValidationResult.Success();
        }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        string? svg = await _apiService.GetScheme(
            settings.City!, settings.Chart!
        );

        if (svg != null)
        {
            string filePath = "scheme.svg";

            try
            {
                await File.WriteAllTextAsync(filePath, svg, Encoding.UTF8);
                Console.WriteLine($"Схема сохранёна в файл: {Path.GetFullPath(filePath)}");
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