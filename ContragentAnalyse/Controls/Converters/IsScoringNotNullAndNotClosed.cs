using ContragentAnalyse.Model.Entities;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ContragentAnalyse.Controls.Converters
{
    public class IsScoringNotNullAndNotClosed : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is Scoring scoring))
            {
                return false;
            }
            return !scoring.IsClosed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
