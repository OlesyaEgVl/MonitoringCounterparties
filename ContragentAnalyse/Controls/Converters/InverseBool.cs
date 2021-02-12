using System;
using System.Globalization;
using System.Windows.Data;

namespace ContragentAnalyse.Controls.Converters
{
    public class InverseBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is bool val))
            {
                return false;
            }
            return !val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool val))
            {
                return false;
            }
            return !val;
        }
    }
}
