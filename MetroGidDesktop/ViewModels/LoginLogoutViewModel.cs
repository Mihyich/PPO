using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetroGid.Controllers.Utility.DTO.Auth;
using MetroGidDesktop.Services.Concrete;
using MetroGidDesktop.Services.Interfaces;

namespace MetroGidDesktop.ViewModels;

public partial class LoginLogoutViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _login = "";

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private bool? _dialogResult;

    public AuthResponseDTO? Response { get; private set; }

    private readonly IAuthService _authService;

    public AsyncRelayCommand LoginLogoutCommand { get; }
    public RelayCommand CancelCommand { get; }

    public LoginLogoutViewModel(IAuthService authService)
    {
        _authService = authService;

        LoginLogoutCommand = new AsyncRelayCommand(ExecuteLoginLogoutAsync);
        CancelCommand = new RelayCommand(() => CloseDialog(false));
    }

    private async Task ExecuteLoginLogoutAsync()
    {
        try
        {
            LoginRequestDTO dto = new(Login, Password);
            Response = await _authService.LogInAsync(dto);

            CloseDialog(true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    private bool ValidateInputs()
    {
        return true;
    }

    private void CloseDialog(bool? result)
    {
        DialogResult = result;
    }
}