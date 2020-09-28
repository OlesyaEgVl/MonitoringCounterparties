using ContragentAnalyse.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using ContragentAnalyse.Model.Implementation;

namespace ContragentAnalyse.Model.Interfaces
{
    public interface IEquationProvider
    {
      //string reader();
        Client GetClient(string bINStr);
        void FillClient(Client client);
        


    }
}
