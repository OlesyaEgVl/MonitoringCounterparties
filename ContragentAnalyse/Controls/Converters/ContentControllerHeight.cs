using System;
using System.Globalization;
using System.Windows.Data;

namespace ContragentAnalyse.Controls.Converters
{
    public class ContentControllerHeight : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is double val) || val <=0)
            {
                return 0;
            }
            return val - 250;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double val))
            {
                return 0;
            }
            return val + 250;
        }
    }
}
