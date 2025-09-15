using MetroGidDesktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MetroGidDesktop.Views;

public partial class AuthView : UserControl
{
    public AuthView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<AuthViewModel>();
    }
}