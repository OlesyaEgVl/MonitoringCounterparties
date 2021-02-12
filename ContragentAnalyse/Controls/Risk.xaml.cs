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
            BankProduct.IsEnabled = false;
            ItogRiskButton.IsEnabled = false;
        }

        private void Redact_Click(object sender, RoutedEventArgs e)
        {
            BankProduct.IsEnabled = true;
        }
    }
}
