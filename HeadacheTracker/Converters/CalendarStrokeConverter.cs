
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

            if (isSelected) return Colors.DeepSkyBlue;  // выбранный день
            if (isToday) return Colors.Red;             // сегодняшний день
            if (hasEntry) return Colors.DarkGreen;      // есть запись
            return Colors.Transparent;                  // обычный день
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
