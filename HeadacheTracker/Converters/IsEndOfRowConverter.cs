
using System;
using System.Globalization;
using Microsoft.Maui.Controls;
namespace HeadacheTracker.Maui.Converters
{
    public class IsEndOfRowConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int index && int.TryParse(parameter?.ToString(), out int span))
        {
            return ((index + 1) % span) == 0;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
}

