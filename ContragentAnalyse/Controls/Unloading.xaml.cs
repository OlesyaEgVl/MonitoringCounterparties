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
    /// Логика взаимодействия для Page4.xaml
    /// </summary>
    public partial class Unloading : UserControl
    {
        public Unloading()
        {
            InitializeComponent();
        }
        private void Search3(object sender, RoutedEventArgs e)
        {
            buttonSearch3.Background = Brushes.DarkGray;
        }
    }
}
