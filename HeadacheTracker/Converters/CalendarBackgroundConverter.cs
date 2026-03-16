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

                // * Приоритет подсветки
                // if (isSelected)
                //    return Colors.Transparent; //  при выборе, альтернатива #E6ECE5 
                //if (isToday)
                //return Colors.Transparent; //  для сегодняшнего дня
                //if (hasEntry)
                //return Colors.Transparent; // если есть запись

                if (isSelected)
                    return Color.FromArgb("#7B9EB1"); // приглушенный синий
                if (isToday)
                    return Color.FromArgb("#D66A5E"); // приглушённый красный
                if (hasEntry)
                    return Color.FromArgb("#70A088"); // приглушённый зелёный

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
