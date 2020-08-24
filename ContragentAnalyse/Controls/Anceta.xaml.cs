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
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Anceta : UserControl
    {
        public Anceta()
        {
            InitializeComponent();
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            buttonSearch.Background = Brushes.DarkGray;
        }
        private void Corr(object sender, RoutedEventArgs e)
        {
            buttonCorr.Background = Brushes.DarkGray;
            buttonWord.Background = Brushes.LightGray;
            buttonSave.Background = Brushes.LightGray;
            TextClientMenedger.IsEnabled = true;
            TextContact.IsEnabled = true;
            CheckBox1.IsEnabled = true;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            buttonCorr.Background = Brushes.LightGray;
            buttonWord.Background = Brushes.LightGray;
            buttonSave.Background = Brushes.DarkGray;
            TextClientMenedger.IsEnabled = false;
            TextContact.IsEnabled = false;
            CheckBox1.IsEnabled = false;
        }

        private void Word(object sender, RoutedEventArgs e)
        {
            buttonCorr.Background = Brushes.LightGray;
            buttonWord.Background = Brushes.LightGray;
            buttonSave.Background = Brushes.LightGray;
        }

        private void XLSX(object sender, RoutedEventArgs e)
        {
            buttonCorr.Background = Brushes.LightGray;
            buttonWord.Background = Brushes.LightGray;
            buttonSave.Background = Brushes.LightGray;
        }
    }

}

