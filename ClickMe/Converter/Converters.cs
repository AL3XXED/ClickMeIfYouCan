using Microsoft.Maui.Graphics; // Wichtig für Colors!
using Microsoft.Maui.Controls; // Für IValueConverter
using System.Globalization;

namespace ClickMe.Converter // Namespace anpassen!
{
    public class PositionToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int position)
            {
                return position switch
                {
                    1 => Colors.Gold,       // Korrekte Syntax: Colors.Gold
                    2 => Colors.Silver,     // Colors.Silver
                    3 => Color.FromArgb("#CD7F32"),     // Colors.Bronze
                    _ => Colors.Transparent // Colors.Transparent
                };
            }

            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
