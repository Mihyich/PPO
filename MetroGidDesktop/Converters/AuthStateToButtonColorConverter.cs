using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace MetroGidDesktop.Converters;

public class AuthStateToButtonColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool isAuthenticated && isAuthenticated ? "#A9A9A9" : "#32CD32";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}