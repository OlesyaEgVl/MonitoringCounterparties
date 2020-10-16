using ContragentAnalyse.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContragentAnalyse.Model.Interfaces
{
    public interface IDataProvider
    {
        DateTime GetDateActual();
       // DateTime? GetDateNextScoring();
        string GetLevelRisk();
        string GetClientManager();
        string GetCriteria();
        string GetContracts();
        bool? GetCardOP();
        string GetAccountNumber();
        string GetContacts();
        IEnumerable<Client> GetClients(string BIN);
        IEnumerable<Client> GetClientsByName(string Name);
        IEnumerable<Criteria> GetCriterias();
        void Commit();
        void AddClient(Client newClient);
        TypeClient GetClientType(string v);
        Country GetCountry(string v);
        Currency GetCurrencyByCode(string code);
        Currency GetCurrencyByName(string name);
        Criteria[] GetCriterialist(string bINStr);
        string AddCriteriaList(Criteria[] criteriaslist); // не уверена, что строку должно возвращать
        IEnumerable<ContactType> GetContactTypes();
    }
}
