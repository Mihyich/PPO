using System.ComponentModel;
using ConsoleClient.Services.Interfaces;
using ConsoleClient.SharedDTO.Auth;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ConsoleClient.Commands.Concrete;

public class LogoutCommand(
    IApiService apiService
) : AsyncCommand<LogoutCommand.Settings>
{
    private readonly IApiService _apiService = apiService;

    public class Settings : CommandSettings
    {
        
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        await _apiService.LogOutAsync();
        return 0;
    }
}