using ContragentAnalyse.Model.Entities;
using ContragentAnalyse.Model.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace ContragentAnalyse.Model.Implementation
{
    public class EFDataProvider : IDataProvider
    {
        readonly DatabaseContext _dbContext;
        public EFDataProvider()
        {
            _dbContext = new DatabaseContext();
        }

        public IEnumerable<Client> GetClients(string BIN) =>
            _dbContext.Client.
            Include(i => i.TypeClient).
            Include(i => i.Actualization).
            Include(i => i.PrescoringScoring).
            Include(i => i.PrescoringScoringHistory).
            Include(i => i.RestrictedAccounts).
            Include(i => i.Contacts).
            Include(i => i.Country).
            Include(i => i.Requests).
            Include(i => i.ClientToCurrency).
                ThenInclude(i => i.Currency).
            Include(i => i.ClientToCriteria).
                ThenInclude(i => i.Criteria).
            Include(i => i.ClientToContracts).
                ThenInclude(i => i.Contracts).
            Where(i => i.BIN.ToLower().Equals(BIN.ToLower()));

        public IEnumerable<Client> GetClientsByName(string Name) => _dbContext.Client.
            Include(i => i.TypeClient).
            Include(i => i.Actualization).
            Include(i => i.PrescoringScoring).
            Include(i => i.PrescoringScoringHistory).
            Include(i => i.RestrictedAccounts).
            Include(i => i.Contacts).
            Include(i => i.Requests).
            Include(i=>i.Country).
            Include(i=>i.ClientToCurrency).
                ThenInclude(i=>i.Currency).
            Include(i => i.ClientToCriteria).
                ThenInclude(i => i.Criteria).
            Include(i => i.ClientToContracts).
                ThenInclude(i => i.Contracts).
            Where(i => i.Name.ToLower().IndexOf(Name.ToLower()) > -1);
        public IEnumerable<Criteria> GetCriterias() => _dbContext.Criteria;
        public IEnumerable<Country> GetCountrys() => _dbContext.Country;
        public IEnumerable<Contracts>  GetCrontracts ()=> _dbContext.Contracts;
        public DateTime GetDateActual()
        {
            Actualization actual = _dbContext.Actualization.Include("Bank").FirstOrDefault(i => i.Id == 0); // Как соеденить таблицы и взять поле. которое нужно
            DateTime DateAct = actual.DateActEKS;
            return DateAct;
        }
        #region

        #endregion

        Currency IDataProvider.GetCurrencyByCode(string code)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                return _dbContext.Currency.FirstOrDefault(i => i.CodeCurrency == code);
            }
            else
            {
                return null;
            }
        }
        Contracts IDataProvider.GetContractByCode(string v)
        { 
            if (!string.IsNullOrWhiteSpace(v))
            {
                return _dbContext.Contracts.FirstOrDefault(cntr => cntr.Name.ToUpper().Equals(v.ToUpper()));
            }
            else { return null; }
        }
        Currency IDataProvider.GetCurrencyByName(string name)
        {
            return _dbContext.Currency.FirstOrDefault(i => i.Name.ToLower().Equals(name.ToLower()));
        }

        public string GetCriterions()
        {
            Criteria criterion = _dbContext.Criteria.Include("Bank").FirstOrDefault(i => i.Id == 0); // Как соеденить таблицы и взять поле. которое нужно
            string Criterions = criterion.Name;
            return Criterions;
        }
        public string GetClientManager()
        {
            Client clients = _dbContext.Client.Include("Bank").FirstOrDefault(i => i.Id == 0); // Как соеденить таблицы и взять поле. которое нужно
            string ClientManagers = clients.ClientManager;
            return ClientManagers;
        }
       public string GetLevelRisk()
        {
            Client clients = _dbContext.Client.Include("Bank").FirstOrDefault(i => i.Id == 0); // Как соеденить таблицы и взять поле. которое нужно
            string ClientManagers = clients.ClientManager;
            return ClientManagers;
        }
        public string GetCriteria()
        {
            Criteria criterias = _dbContext.Criteria.Include("CriteriaToScoring").FirstOrDefault(i => i.Id == 0);

            string Crit = criterias.Name;
            return Crit;
        }
        public string GetContracts()
        {
            Contracts actcontract = _dbContext.Contracts.Include("Bank").FirstOrDefault(i => i.Id == 0);
            string Contract = actcontract.Name;
            return Contract;
        }
        public bool? GetCardOP()
        {
            Client card = _dbContext.Client.Include("Bank").FirstOrDefault(i => i.Id == 0);
            bool? CardOP = card.CardOP;
            return CardOP;
        }
        public string GetAccountNumber()
        {
            RestrictedAccounts AccountNumber = _dbContext.RestrictedAccounts.Include("Bank").FirstOrDefault(i => i.Id == 0); // Как соеденить таблицы и взять поле. которое нужно
            string AccountNumbers = AccountNumber.AccountNumber;
            return AccountNumbers;
        }
        public string GetContacts()
        {
            Contacts AccountNumber = _dbContext.Contacts.Include("Bank").FirstOrDefault(i => i.Id == 0); // Как соеденить таблицы и взять поле. которое нужно
            string AccountNumbers = AccountNumber.ContactFIO;
            return AccountNumbers;
        }
        public void Commit()
        {
            _dbContext.SaveChanges();
        }

        public IEnumerable<ContactType> GetContactTypes()
        {
            return _dbContext.ContactType;
        }
        public void AddDateNextScoring(Client newClient)
        {
            _dbContext.Client.Add(newClient);
            _dbContext.SaveChanges();
        }
        public void AddClient(Client newClient)
        {
            if (_dbContext.Client.Any(client => client.BIN.Equals(newClient.BIN)))
            {
                MessageBox.Show("Клиент уже существует!");
            }
            else
            {
                _dbContext.Client.Add(newClient);
                _dbContext.SaveChanges();
                MessageBox.Show("Клиент добавлен!");
            }
        }
        string [] BankTypeCodes = new string [] { "HA","HB","HD","HU","HC"};
        string[] LLCTypeCodes = new string[] { "HA" };

        public TypeClient GetClientType(string v)
        {
            if (BankTypeCodes.Any(i => i.Equals(v)))
            {
                return _dbContext.TypeClient.FirstOrDefault(i => i.Name.Equals("Банк"));
            }
            if (LLCTypeCodes.Any(i => i.Equals(v)))
            {
                return _dbContext.TypeClient.FirstOrDefault(i => i.Name.Equals("Юрлицо"));
            }
            return null;
        }

        Country IDataProvider.GetCountry(string v)
        {
            if (!string.IsNullOrWhiteSpace(v))
            {
                return _dbContext.Country.FirstOrDefault(cntr => cntr.Code.ToUpper().Equals(v.ToUpper()));
            }
            else { return null; }
        }

        public Criteria[] GetCriterialist(string bINStr)
        {
            throw new NotImplementedException();
        }

        public Employees GetCurrentEmployee()
        {
            string login = Environment.UserName;
            if (_dbContext.Employees.Any(i => i.CodeName.ToLower().Equals(Environment.UserName.ToLower())))
            {
                return _dbContext.Employees.FirstOrDefault(i => i.CodeName.ToLower().Equals(login.ToLower()));
            }
            else
            {
                Employees newEmpl = new Employees();
                newEmpl.CodeName = login;
                _dbContext.Employees.Add(newEmpl);
                _dbContext.SaveChanges();
                return newEmpl;
            }
        }
    }
}
