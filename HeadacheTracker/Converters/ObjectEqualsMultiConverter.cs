using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace HeadacheTracker.Maui.Converters
{
    public class ObjectEqualsMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return false;

            var selectedItem = values[0];
            var currentItem = values[1];

            if (selectedItem == null && currentItem == null)
                return true;

            if (selectedItem == null || currentItem == null)
                return false;

            return Equals(selectedItem, currentItem);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
