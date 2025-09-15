using Avalonia.Interactivity;
using MetroGidDesktop.ViewModels;

namespace MetroGidDesktop.Views;

public partial class RegisterDialog : Window
{
    public RegisterDialog(RegisterViewModel registerViewModel)
    {
        InitializeComponent();
        DataContext = registerViewModel;

        ((INotifyPropertyChanged)DataContext).PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(RegisterViewModel.DialogResult))
        {
            var viewModel = (RegisterViewModel)sender!;

            if (viewModel.DialogResult.HasValue)
                Close(viewModel.DialogResult.Value);
        }
    }

    private void OnRegisterClick(object sender, RoutedEventArgs e)
    {
        (DataContext as RegisterViewModel)?.RegisterCommand.Execute(null);
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        Close(false);
    }
}