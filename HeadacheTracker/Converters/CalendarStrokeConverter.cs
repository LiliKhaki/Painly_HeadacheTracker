
using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace HeadacheTracker.Maui.Converters
{
    public class CalendarStrokeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool hasEntry = values[0] is bool h && h;
            bool isToday = values[1] is bool t && t;
            bool isSelected = values[2] is bool s && s;

            if (isSelected)
                return Color.FromArgb("#7B9EB1"); // приглушенный синий
            if (isToday)
                return Color.FromArgb("#D66A5E"); // приглушённый красный
            if (hasEntry)
                return Color.FromArgb("#4FAE7C"); // приглушённый зелёный
            return Colors.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
