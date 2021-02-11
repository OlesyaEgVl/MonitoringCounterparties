using ContragentAnalyse.Model.Entities;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ContragentAnalyse.Controls.Converters
{
    public class ClientTotalRiskLevel : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is Client client))
            {
                return null;
            }
            double riskLevel = client.Scorings.Select(i => i.LoroRiskLevel + i.NostroRiskLevel).Sum();
            string result = riskLevel switch
            {
                double n when n > 13.1 => $"{riskLevel:N1} - Критичный",
                double n when n >= 5.6 && n <= 13.1 => $"{riskLevel:N1} - Высокий",
                double n when n >= 3.5 && n <= 5.5 => $"{riskLevel:N1} - Средний",
                double n when n <= 3.4 => $"{riskLevel:N1} - Низкий",
                _ => "Не определено",
            };
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
