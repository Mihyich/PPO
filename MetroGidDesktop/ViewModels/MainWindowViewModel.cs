using CommunityToolkit.Mvvm.ComponentModel;
using MetroGidDesktop.Services.Interfaces;

namespace MetroGidDesktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IAuthState _authState;
    private readonly IAuthService _authService;

    public MainWindowViewModel(
        IAuthState authState,
        IAuthService authService,
        AuthViewModel authViewModel
        )
    {
        _authState = authState;
        _authService = authService;
    }
}