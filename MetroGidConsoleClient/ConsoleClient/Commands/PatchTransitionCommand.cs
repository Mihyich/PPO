using System.ComponentModel;
using ConsoleClient.Services.Interfaces;
using ConsoleClient.SharedDTO.Auth;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ConsoleClient.Commands.Concrete;

public class PatchTransitionCommand(
    IApiService apiService
) : AsyncCommand<PatchTransitionCommand.Settings>
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

        [CommandOption("--new-occupancy")]
        [Description("Новая загруженность перехода")]
        public string? NewOccupancy { get; set; }

        [CommandOption("--new-access")]
        [Description("Новый тип доступа перехода")]
        public string? NewAccess { get; set; }

        [CommandOption("--new-duration")]
        [Description("Новая средняя продолжительность перехода")]
        public string? NewDuration { get; set; }

        [CommandOption("--new-opentime")]
        [Description("Новое время открытия перехода")]
        public string? NewOpenTime { get; set; }

        [CommandOption("--new-closetime")]
        [Description("Новое время закрытия перехода")]
        public string? NewCloseTime { get; set; }

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

            if (string.IsNullOrWhiteSpace(NewOccupancy))
                return ValidationResult.Error("Требуется --new-occupany");

            if (string.IsNullOrWhiteSpace(NewAccess))
                return ValidationResult.Error("Требуется --new-access");

            if (string.IsNullOrWhiteSpace(NewDuration))
                return ValidationResult.Error("Требуется --new-duration");

            if (string.IsNullOrWhiteSpace(NewOpenTime))
                return ValidationResult.Error("Требуется --new-opentime|");

            if (string.IsNullOrWhiteSpace(NewCloseTime))
                return ValidationResult.Error("Требуется --new-closetime|");

            return ValidationResult.Success();
        }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        int.TryParse(settings.NewOccupancy!, out int occup);

        await _apiService.PatchTransition(
            settings.City!, settings.Chart!,
            settings.FromBranch!, settings.FromStation!,
            settings.ToBranch!, settings.ToStation!,
            occup, settings.NewAccess!, settings.NewDuration!,
            settings.NewOpenTime!, settings.NewCloseTime!
        );
        
        return 0;
    }
}