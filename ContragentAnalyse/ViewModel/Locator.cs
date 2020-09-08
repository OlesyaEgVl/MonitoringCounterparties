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
