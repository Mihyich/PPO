using System;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using MetroGidDesktop.ViewModels;

namespace MetroGidDesktop.Views;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var viewModelName = param.GetType().FullName!;
        var viewName = viewModelName.Replace("ViewModel", "View", StringComparison.Ordinal);
        var assembly = Assembly.GetExecutingAssembly();
        var viewType = assembly.GetType(viewName);

        if (viewType != null && typeof(Control).IsAssignableFrom(viewType))
            return (Control)Activator.CreateInstance(viewType)!;

        return new TextBlock { Text = $"View not found: {viewName}" };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
