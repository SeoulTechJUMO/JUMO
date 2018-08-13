using System;
using System.Globalization;
using System.Windows.Data;

namespace JUMO.UI
{
    [ValueConversion(typeof(double), typeof(double))]
    class MinusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => -System.Convert.ToDouble(value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => -System.Convert.ToDouble(value);
    }
}
