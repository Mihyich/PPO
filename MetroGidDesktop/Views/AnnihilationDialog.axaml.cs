using Avalonia.Interactivity;
using MetroGidDesktop.ViewModels;

namespace MetroGidDesktop.Views;

public partial class AnnihilationDialog : Window
{
    public AnnihilationDialog(AnnihilationViewModel annihilationViewModel)
    {
        InitializeComponent();
        DataContext = annihilationViewModel;

        ((INotifyPropertyChanged)DataContext).PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AnnihilationViewModel.DialogResult))
        {
            var viewModel = (AnnihilationViewModel)sender!;

            if (viewModel.DialogResult.HasValue)
                Close(viewModel.DialogResult.Value);
        }
    }

    private void OnAnnihilationClick(object sender, RoutedEventArgs e)
    {
        (DataContext as AnnihilationViewModel)?.AnnihilationCommand.Execute(null);
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        Close(false);
    }
}