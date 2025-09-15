using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetroGid.Controllers.Utility.DTO.Auth;
using MetroGidDesktop.Services.Interfaces;

namespace MetroGidDesktop.ViewModels;

public partial class AnnihilationViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private bool? _dialogResult;

    private readonly IAuthService _authService;

    public AsyncRelayCommand AnnihilationCommand { get; }
    public RelayCommand CancelCommand { get; }

    public AnnihilationViewModel(IAuthService authService)
    {
        _authService = authService;

        AnnihilationCommand = new AsyncRelayCommand(ExecuteAnnihilationAsync);
        CancelCommand = new RelayCommand(() => CloseDialog(false));
    }

    private async Task ExecuteAnnihilationAsync()
    {
        if (!ValidateInputs())
            return;

        try
        {
            // await Task.Delay(500);

            CloseDialog(true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка удаления аккаунта: {ex.Message}");
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