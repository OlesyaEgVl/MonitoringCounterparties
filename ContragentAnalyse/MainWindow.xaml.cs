using ContragentAnalyse.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ContragentAnalyse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuClick(object sender, RoutedEventArgs e)
        {
            string currentTag = (sender as Button).Tag.ToString();
            switch (currentTag)
            {
                case ("Page1"):
                    MainContentController.Content = new Anceta();
                    break;
                /*case ("Page2"):
                    MainContentController.Content = new Page2();
                    break;*/
                case ("Page3"):
                    MainContentController.Content = new Risk();
                    break;
                case ("Page4"):
                    MainContentController.Content = new Unloading();
                    break;
            }
            
        }
    }
}
