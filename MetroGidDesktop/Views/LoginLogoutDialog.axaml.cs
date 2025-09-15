using Avalonia.Interactivity;
using MetroGidDesktop.ViewModels;

namespace MetroGidDesktop.Views;

public partial class LoginLogoutDialog : Window
{
    public LoginLogoutDialog(LoginLogoutViewModel LoginLogoutViewModel)
    {
        InitializeComponent();
        DataContext = LoginLogoutViewModel;

        ((INotifyPropertyChanged)DataContext).PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LoginLogoutViewModel.DialogResult))
        {
            var viewModel = (LoginLogoutViewModel)sender!;

            if (viewModel.DialogResult.HasValue)
                Close(viewModel.DialogResult.Value);
        }
    }

    private void OnLoginClick(object sender, RoutedEventArgs e)
    {
        (DataContext as LoginLogoutViewModel)?.LoginLogoutCommand.Execute(null);
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        Close(false);
    }
}