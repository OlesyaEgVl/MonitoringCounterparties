using ContragentAnalyse.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContragentAnalyse.Model.Interfaces
{
    interface IDataProvider
    {
        Bank GetBank(string BIN);
        DateTime GetDateActual();
        DateTime? GetDateNextScoring();
        string GetLevelRisk();
        string GetClientManager();
        string GetCriteria();
        string GetContracts();
        bool? GetCardOP();
        string GetAccountNumber();
        string GetContacts();

        List<Client> GetClients(string BIN);
        void Commit();
    }
}
