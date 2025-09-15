using CommunityToolkit.Mvvm.ComponentModel;
using MetroGidDesktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MetroGidDesktop.Views;

public partial class RouteView : UserControl
{
    public RouteView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<RouteViewModel>();
    }
}