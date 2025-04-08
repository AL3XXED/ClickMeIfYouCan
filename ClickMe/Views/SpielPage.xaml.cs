using System.Globalization;
using ClickMe.ViewModels;

namespace ClickMe.Views;

public partial class SpielPage : ContentPage
{
  
    public SpielPage()
    {
        InitializeComponent();
    }

}
public class XToRectConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double x = (double)value;
        double y = parameter != null ? double.Parse(parameter.ToString()) : 0;
        return new Rect(x, y, 80, 80); 
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
