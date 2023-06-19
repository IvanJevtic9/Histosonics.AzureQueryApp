using System;
using System.Globalization;
using System.Windows.Data;

namespace Histosonics.AzureQueryApp.Conventers
{
    [ValueConversion(typeof(double), typeof(string))]
    public class ProgressBarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() + " %";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
