using System.ComponentModel;
using ConsoleClient.Services.Interfaces;
using ConsoleClient.SharedDTO.Auth;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ConsoleClient.Commands.Concrete;

public class PatchBranchCommand(
    IApiService apiService
) : AsyncCommand<PatchBranchCommand.Settings>
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

        [CommandOption("--new-branch")]
        [Description("Новое название ветки")]
        public string? NewBranch { get; set; }

        [CommandOption("--new-color")]
        [Description("Новый цвет ветки")]
        public string? NewColor { get; set; }

        [CommandOption("--new-access")]
        [Description("Новый тип доступа")]
        public string? NewAccess { get; set; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(City))
                return ValidationResult.Error("Требуется --city|-c");

            if (string.IsNullOrWhiteSpace(Chart))
                return ValidationResult.Error("Требуется --chart|-ch");

            if (string.IsNullOrWhiteSpace(Branch))
                return ValidationResult.Error("Требуется --branch|-b");

            if (string.IsNullOrWhiteSpace(NewBranch))
                return ValidationResult.Error("Требуется --new-branch|-nb");

            if (string.IsNullOrWhiteSpace(NewColor))
                return ValidationResult.Error("Требуется --new-color|-nc");

            if (string.IsNullOrWhiteSpace(NewAccess))
                return ValidationResult.Error("Требуется --new-access|-na");

            return ValidationResult.Success();
        }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        await _apiService.PatchBranch(
            settings.City!, settings.Chart!, settings.Branch!,
            settings.NewBranch!, settings.NewColor!, settings.NewAccess!);
        return 0;
    }
}