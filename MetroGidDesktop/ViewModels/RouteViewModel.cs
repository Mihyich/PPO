using CommunityToolkit.Mvvm.ComponentModel;

namespace MetroGidDesktop.ViewModels;

public partial class RouteViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _svgContent = "";

    public RouteViewModel()
    {

    }
}