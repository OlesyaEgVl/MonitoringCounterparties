using ContragentAnalyse.Model.Implementation;
using ContragentAnalyse.Model.Interfaces;

namespace ContragentAnalyse.ViewModel
{
    public class Locator
    {

        readonly IDataProvider provider;
        readonly IEquationProvider eqProvider;
        public MainViewModel Main
        {
            get
            {
                return MainViewModel.GetInstance(provider, eqProvider);
            }
        }

        public Locator()
        {
            provider = new EFDataProvider();
            eqProvider = new EquationProvider(provider);
        }
    }
}