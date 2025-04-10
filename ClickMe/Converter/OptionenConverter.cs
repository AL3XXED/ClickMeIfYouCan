using Microsoft.Maui.Controls;
using System.Globalization;

namespace ClickMe.Converter
{
    public class OptionenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == parameter?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
                return int.Parse((string)parameter);

            return Binding.DoNothing;
        }
    }
}
