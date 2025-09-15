using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MetroGid.Controllers.Utility.DTO.Auth;
using MetroGidDesktop.Commands;
using MetroGidDesktop.Services.Concrete;
using MetroGidDesktop.Services.Interfaces;
using MetroGidDesktop.Views;
using Microsoft.Extensions.DependencyInjection;

namespace MetroGidDesktop.ViewModels;

public class AuthViewModel : ViewModelBase
{
    private readonly IServiceProvider _serviceProvider;

    private AuthResponseDTO? _user;

    public AuthResponseDTO? User
    {
        get => _user;
        set
        {
            _user = value;
            OnPropertyChanged(nameof(User));
            OnPropertyChanged(nameof(IsAuthenticated));
            OnPropertyChanged(nameof(Login));
            OnPropertyChanged(nameof(Role));
        }
    }


    // связка с AXAML
    public string Login => User?.Login ?? "Гость";
    public string Role => User?.Role ?? "UNSIGNED";
    public bool IsAuthenticated => User != null;

    public AsyncRelayCommand LoginLogoutCommand { get; }
    public AsyncRelayCommand RegisterCommand { get; }
    public AsyncRelayCommand AnnihilationCommand { get; }

    public AuthViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        LoginLogoutCommand = new AsyncRelayCommand(ExecuteLoginLogoutAsync);
        RegisterCommand = new AsyncRelayCommand(ExecuteRegisterAsync);
        AnnihilationCommand = new AsyncRelayCommand(ExecuteAnnihilationAsync);
    }

    private async Task ExecuteLoginLogoutAsync()
    {
        if (IsAuthenticated)
            await ExecuteLogoutAsync();
        else
            await ExecuteLoginAsync();
    }

    private async Task ExecuteLoginAsync()
    {
        var loginlogoutVm = new LoginLogoutViewModel(_serviceProvider.GetRequiredService<IAuthService>());
        var dialog = new LoginLogoutDialog(loginlogoutVm);

        var result = await dialog.ShowDialog<bool?>(App.MainWindow!);

        if (result == true)
            User = loginlogoutVm.Response;
    }

    private async Task ExecuteLogoutAsync()
    {
        User = null;
    }

    private async Task ExecuteRegisterAsync()
    {
        var registerVm = new RegisterViewModel(_serviceProvider.GetRequiredService<IAuthService>());
        var dialog = new RegisterDialog(registerVm);

        var result = await dialog.ShowDialog<bool?>(App.MainWindow!);

        if (result == true)
        {
            User = new AuthResponseDTO(
                Id: new Random().Next(1000, 9999),
                Token: Guid.NewGuid().ToString(),
                Login: registerVm.Login,
                Role: registerVm.Role
            );
        }
    }

    private async Task ExecuteAnnihilationAsync()
    {
        var annihilationVm = new AnnihilationViewModel(_serviceProvider.GetRequiredService<IAuthService>());
        var dialog = new AnnihilationDialog(annihilationVm);

        var result = await dialog.ShowDialog<bool?>(App.MainWindow!);
    }
}