using System.ComponentModel;
using ConsoleClient.Services.Interfaces;
using ConsoleClient.SharedDTO.Auth;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ConsoleClient.Commands.Concrete;

public class PatchStationCommand(
    IApiService apiService
) : AsyncCommand<PatchStationCommand.Settings>
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

        [CommandOption("--station|-s")]
        [Description("Станция")]
        public string? Station { get; set; }

        [CommandOption("--new-station")]
        [Description("Новое название станции")]
        public string? NewStation { get; set; }

        [CommandOption("--new-occupancy")]
        [Description("Новое загруженность станции")]
        public string? NewOccupancy { get; set; }

        [CommandOption("--new-access")]
        [Description("Новый тип доступа станции")]
        public string? NewAccess { get; set; }

        [CommandOption("--new-opentime")]
        [Description("Новое время открытия станции")]
        public string? NewOpenTime { get; set; }

        [CommandOption("--new-closetime")]
        [Description("Новое время закрытия станции")]
        public string? NewCloseTime { get; set; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(City))
                return ValidationResult.Error("Требуется --city|-c");

            if (string.IsNullOrWhiteSpace(Chart))
                return ValidationResult.Error("Требуется --chart|-ch");

            if (string.IsNullOrWhiteSpace(Branch))
                return ValidationResult.Error("Требуется --branch|-b");

            if (string.IsNullOrWhiteSpace(Station))
                return ValidationResult.Error("Требуется --station|-s");

            if (string.IsNullOrWhiteSpace(NewStation))
                return ValidationResult.Error("Требуется --new-station|-ns");

            if (string.IsNullOrWhiteSpace(NewOccupancy))
                return ValidationResult.Error("Требуется --new-occupany|-no");

            if (string.IsNullOrWhiteSpace(NewAccess))
                return ValidationResult.Error("Требуется --new-access|-na");

            if (string.IsNullOrWhiteSpace(NewOpenTime))
                return ValidationResult.Error("Требуется --new-opentime|-not");

            if (string.IsNullOrWhiteSpace(NewCloseTime))
                return ValidationResult.Error("Требуется --new-closetime|-nct");

            return ValidationResult.Success();
        }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        int.TryParse(settings.NewOccupancy!, out int occup);

        await _apiService.PatchStation(
            settings.City!, settings.Chart!, settings.Branch!, settings.Station!,
            settings.NewStation!, occup, settings.NewAccess!,
            settings.NewOpenTime!, settings.NewCloseTime!
        );

        return 0;
    }
}