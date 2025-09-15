using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using MetroGidDesktop.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MetroGidDesktop.Services.Concrete;

public class DialogService : IDialogService
{
    private readonly IServiceProvider _serviceProvider;

    public DialogService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<bool?> ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : class
    {
        var viewTypeName = typeof(TViewModel).Name.Replace("ViewModel", "Dialog");
        var viewType = Type.GetType($"MetroGidDesktop.Views.{viewTypeName}");

        if (viewType == null)
            throw new InvalidOperationException($"View not found for ViewModel: {typeof(TViewModel).Name}");

        var dialog = _serviceProvider.GetRequiredService(viewType) as Window;

        if (dialog == null)
            throw new InvalidOperationException($"Failed to resolve view: {viewType.Name}");

        dialog.DataContext = viewModel;

        return await dialog.ShowDialog<bool?>(null);
    }
}