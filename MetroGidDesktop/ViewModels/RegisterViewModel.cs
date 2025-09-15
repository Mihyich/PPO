using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetroGid.Controllers.Utility.DTO.Auth;
using MetroGidDesktop.Services.Interfaces;

namespace MetroGidDesktop.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _login = "";

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private string _confirmPassword = "";

    [ObservableProperty]
    private string _role = "Пользователь"; // можно сделать выпадающим списком позже

    [ObservableProperty]
    private bool? _dialogResult;

    private readonly IAuthService _authService;

    public AsyncRelayCommand RegisterCommand { get; }
    public RelayCommand CancelCommand { get; }

    public RegisterViewModel(IAuthService authService)
    {
        _authService = authService;

        RegisterCommand = new AsyncRelayCommand(ExecuteRegisterAsync);
        CancelCommand = new RelayCommand(() => CloseDialog(false));
    }

    private async Task ExecuteRegisterAsync()
    {
        if (!ValidateInputs())
            return;

        try
        {
            // await Task.Delay(500);

            var newUser = new AuthResponseDTO(
                Id: new Random().Next(1000, 9999),
                Token: Guid.NewGuid().ToString(),
                Login: Login,
                Role: Role
            );

            CloseDialog(true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка регистрации: {ex.Message}");
        }
    }

    private bool ValidateInputs()
    {
        return true;

        if (string.IsNullOrWhiteSpace(Login))
        {
            // Можно показать сообщение пользователю через INotifyDataErrorInfo (опционально)
            return false;
        }

        if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
        {
            return false;
        }

        if (Password != ConfirmPassword)
        {
            return false;
        }

        return true;
    }

    private void CloseDialog(bool? result)
    {
        DialogResult = result;
    }
}