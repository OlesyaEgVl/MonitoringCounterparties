using System;
using System.Collections.Generic;
using System.Text;

namespace ContragentAnalyse.ViewModel
{
    public class MainViewModel
    {
        private static MainViewModel instance;
        private MainViewModel()
        {

        }

        public static MainViewModel GetInstance()
        {
            instance ??= new MainViewModel();
            return instance;
        }
    }
}
