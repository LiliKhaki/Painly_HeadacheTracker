using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace HeadacheTracker.Maui.Converters
{
    public class CalendarBackgroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values.Length < 3)
                    return Colors.Transparent;

                bool hasEntry = values[0] is bool b1 && b1;
                bool isToday = values[1] is bool b2 && b2;
                bool isSelected = values[2] is bool b3 && b3;

                // Приоритет подсветки
                if (isSelected)
                    return Color.FromArgb("#CCE0FF"); // светло-синий фон при выборе
                if (isToday)
                    return Color.FromArgb("#FFF4E6"); // мягкий оранжевый фон для сегодняшнего дня
                if (hasEntry)
                    return Color.FromArgb("#FFE6E6"); // розоватый фон, если есть запись

                return Colors.Transparent;
            }
            catch
            {
                return Colors.Transparent;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
