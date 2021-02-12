using ContragentAnalyse.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContragentAnalyse.Model.Interfaces
{
    public interface IDataProvider
    {
        IEnumerable<Client> GetClients(string BIN);
        IEnumerable<Client> GetClients();
        IEnumerable<Client> GetClientsByName(string Name);
        IEnumerable<Criteria> GetCriterias();
        IEnumerable<Country> GetCountries();
        bool IsAnyClientExist(string Bin);
        void Commit();
        void AddScoring(Scoring newScoring);
        void AddClient(Client newClient);
        ClientType GetClientType(string v);
        Country GetCountry(string v);
        Currency GetCurrencyByCode(string code);
        IEnumerable<ContactType> GetContactTypes();
        Contracts GetContractByCode(string v);
        Employee GetCurrentEmployee();
        void SaveScoring(Scoring scoring);
    }
}
