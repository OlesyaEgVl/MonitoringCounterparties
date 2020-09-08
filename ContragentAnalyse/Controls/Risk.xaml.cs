using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ContragentAnalyse.Controls
{
    /// <summary>
    /// Interaction logic for Page3.xaml
    /// </summary>
    public partial class Risk : UserControl
    {
        public Risk()
        {
            InitializeComponent();
        }

        private void SaveHistoryTime_Click(object sender, RoutedEventArgs e)
        {
            RedactHistory.Background = Brushes.LightGray;
            SaveHistoryTime.Background = Brushes.DarkGray;
            BankProduct.IsEnabled = false;
            ItogRiskButton.IsEnabled = false;
            ItogRisk.IsEnabled = false;
        }

        private void Redact_Click(object sender, RoutedEventArgs e)
        {
            RedactHistory.Background = Brushes.DarkGray;
            SaveHistoryTime.Background = Brushes.LightGray;
            BankProduct.IsEnabled = true;
            ItogRiskButton.IsEnabled = true;
            ItogRisk.IsEnabled = true;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RedactHistory.Background = Brushes.LightGray;
            SaveHistoryTime.Background = Brushes.LightGray;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RedactHistory.Background = Brushes.LightGray;
            SaveHistoryTime.Background = Brushes.LightGray;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            DateRevision.IsEnabled = true;
        }
    }
}
