using ContragentAnalyse.Model.Entities;

namespace ContragentAnalyse.Model.Interfaces
{
    public interface IEquationProvider
    {
        Client GetClient(string bINStr);
        void FillClient(Client client);
    }
}
