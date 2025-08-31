using System.ComponentModel;
using ConsoleClient.Services.Interfaces;
using ConsoleClient.SharedDTO.Auth;
using MetroGid.Controllers.Utility.DTO.Concrete;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ConsoleClient.Commands.Concrete;

public class GetChartCitiesTitlesCommand(
    IApiService apiService
) : AsyncCommand<GetChartCitiesTitlesCommand.Settings>
{
    private readonly IApiService _apiService = apiService;

    public class Settings : CommandSettings
    {
        
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        List<ChartDTO> charts = await _apiService.GetChartsCitiesTitles();

        Table table = new Table()
                .RoundedBorder()
                .AddColumn("Город")
                .AddColumn("Схема")
                .AddEmptyRow();

        foreach (var chart in charts)
            table.AddRow(chart.City, chart.Title).AddEmptyRow();

        AnsiConsole.Write(table);
        return 0;
    }
}