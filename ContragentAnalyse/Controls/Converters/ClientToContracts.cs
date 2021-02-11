using ContragentAnalyse.Model.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ContragentAnalyse.Controls.Converters
{
    public class ClientToContracts : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is Client client))
            {
                return string.Empty;
            }
            return string.Join(", ", client.ClientToContracts.Select(i => i.Contracts.Name));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
