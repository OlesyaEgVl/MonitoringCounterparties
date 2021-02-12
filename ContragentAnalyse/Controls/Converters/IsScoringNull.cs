using ContragentAnalyse.Model.Entities;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ContragentAnalyse.Controls.Converters
{
    public class IsScoringNull : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is Scoring client))
            {
                return false;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
