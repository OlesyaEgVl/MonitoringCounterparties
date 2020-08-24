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
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class NewClients : UserControl
    {
        public NewClients()
        {
            InitializeComponent();
        }
        
private void New_Click(object sender, RoutedEventArgs e)
        {
            New.Background = Brushes.DarkGray;
        }
    }
}
