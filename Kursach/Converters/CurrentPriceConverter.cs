using System.Globalization;
using System.Windows.Data;
using Kursach.Model.Models;

namespace Kursach.Converters;

public class CurrentPriceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Product p)
        {
            if (p.DiscountedPrice.HasValue)
                return p.DiscountedPrice.Value.ToString("F2", culture);

            return p.Price.ToString("F2", culture);
        }

        return "0.00";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}