using System;
using System.Globalization;
using System.Windows.Data;

namespace NumMethods2.XamlConverters
{
    internal class ArraySizeTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{value} x {value}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}