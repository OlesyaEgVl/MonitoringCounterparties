using ContragentAnalyse.Model.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace ContragentAnalyse.Controls.Converters
{
    public class ClientLatestLoroRiskLevel : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is Client client) || client.Scorings.Count == 0)
            {
                return null;
            }
            return client.Scorings[client.Scorings.Count - 1].LoroRiskLevel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
