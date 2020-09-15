using ContragentAnalyse.Model.Implementation;
using ContragentAnalyse.Model.Interfaces;

namespace ContragentAnalyse.ViewModel
{
    public class Locator
    {
        readonly IDataProvider provider = new EFDataProvider();
        public MainViewModel Main
        {
            get
            {
                return MainViewModel.GetInstance(provider);
            }
        }
    }
}