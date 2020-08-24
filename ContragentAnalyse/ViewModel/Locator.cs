using System;
using System.Collections.Generic;
using System.Text;

namespace ContragentAnalyse.ViewModel
{
    public class Locator
    {
        public MainViewModel Main
        {
            get
            {
                return MainViewModel.GetInstance();
            }
        }
    }
}
