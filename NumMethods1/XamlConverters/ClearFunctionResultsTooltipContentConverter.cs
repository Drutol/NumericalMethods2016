using System;
using System.Globalization;
using System.Windows.Data;
using NumMethods1.ViewModels;

namespace NumMethods1.XamlConverters
{
    internal class ClearFunctionResultsTooltipContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{ViewModelLocator.Main.Locale["#ResetResultsFor"]} {value}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}