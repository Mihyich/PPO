using System.ComponentModel;
using ConsoleClient.Services.Interfaces;
using ConsoleClient.SharedDTO.Auth;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ConsoleClient.Commands.Concrete;

public class PatchRailwayCommand(
    IApiService apiService
) : AsyncCommand<PatchRailwayCommand.Settings>
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

        [CommandOption("--branch|-b")]
        [Description("Ветка")]
        public string? Branch { get; set; }

        [CommandOption("--duty-station")]
        [Description("Дежурная станция <C>")]
        public string? DutyStation { get; set; }

        [CommandOption("--to-station")]
        [Description("Cтанция <На>")]
        public string? ToStation { get; set; }

        [CommandOption("--new-duration")]
        [Description("Новая средняя продолжительность перехода")]
        public string? NewDuration { get; set; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(City))
                return ValidationResult.Error("Требуется --city|-c");

            if (string.IsNullOrWhiteSpace(Chart))
                return ValidationResult.Error("Требуется --chart|-ch");

            if (string.IsNullOrWhiteSpace(DutyStation))
                return ValidationResult.Error("Требуется --duty-station|-ds");

            if (string.IsNullOrWhiteSpace(ToStation))
                return ValidationResult.Error("Требуется --to-station|-ts");

            if (string.IsNullOrWhiteSpace(NewDuration))
                return ValidationResult.Error("Требуется --new-duration|-nd");

            return ValidationResult.Success();
        }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        await _apiService.PatchRailway(
            settings.City!, settings.Chart!,
            settings.Branch!, settings.DutyStation!, settings.ToStation!,
            settings.NewDuration!
        );
        
        return 0;
    }
}