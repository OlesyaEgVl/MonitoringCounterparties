using ContragentAnalyse.Model.Entities;
using ContragentAnalyse.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
            Include(i => i.Scorings).ThenInclude(i => i.Employee).
            Include(i=>i.Scorings).ThenInclude(s=>s.Criterias).ThenInclude(c => c.Criteria).
            Include(i => i.RestrictedAccounts).
            Include(i => i.Contacts).
            Include(i => i.Country).
            Include(i => i.Requests).
            Include(i => i.ClientToCurrency).ThenInclude(i => i.Currency).
            Include(i => i.ClientToContracts).ThenInclude(i => i.Contracts).
            Where(i => i.BIN.ToLower().Equals(BIN.ToLower())).
            ToList();

        public IEnumerable<Client> GetClients() =>
            _dbContext.Client.
            Include(i => i.TypeClient).
            Include(i => i.Actualization).
            Include(i => i.Scorings).ThenInclude(i => i.Employee).
            Include(i => i.Scorings).ThenInclude(s => s.Criterias).ThenInclude(c => c.Criteria).
            Include(i => i.RestrictedAccounts).
            Include(i => i.Contacts).
            Include(i => i.Country).
            Include(i => i.Requests).
            Include(i => i.ClientToCurrency).ThenInclude(i => i.Currency).
            Include(i => i.ClientToContracts).ThenInclude(i => i.Contracts).
            ToList();

        public IEnumerable<Client> GetClientsByName(string Name) => _dbContext.Client.
            Include(i => i.TypeClient).
            Include(i => i.Actualization).
            Include(i => i.Scorings).ThenInclude(i => i.Employee).
            Include(i => i.Scorings).ThenInclude(s => s.Criterias).ThenInclude(c=>c.Criteria).
            Include(i => i.RestrictedAccounts).
            Include(i => i.Contacts).
            Include(i => i.Country).
            Include(i => i.Requests).
            Include(i => i.ClientToCurrency).ThenInclude(i => i.Currency).
            Include(i => i.ClientToContracts).ThenInclude(i => i.Contracts).
            Where(i => i.FullName.ToLower().IndexOf(Name.ToLower()) > -1).ToList();

        public IEnumerable<Criteria> GetCriterias() => _dbContext.Criteria;
        public IEnumerable<Country> GetCountries() => _dbContext.Country;
       
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

        public bool IsAnyClientExist(string Bin)
        {
            return _dbContext.Client.Any(i => i.BIN.ToLower().Equals(Bin.ToLower()));
        }

        Contracts IDataProvider.GetContractByCode(string v)
        {
            if (string.IsNullOrWhiteSpace(v))
            {
                return null;
            }
            if (_dbContext.Contracts.Any(i => i.Name.ToLower().Equals(v.ToLower())))
            {
                return _dbContext.Contracts.First(cntr => cntr.Name.ToUpper().Equals(v.ToUpper()));
            }
            else
            {
                Contracts contracts = new Contracts
                {
                    Name = v
                };
                _dbContext.Contracts.Add(contracts);
                _dbContext.SaveChanges();
                return contracts;
            }

        }
        
        public void Commit()
        {
            _dbContext.SaveChanges();
        }

        public IEnumerable<ContactType> GetContactTypes()
        {
            return _dbContext.ContactType;
        }

        public void AddClient(Client newClient)
        {
            if (_dbContext.Client.Any(client => client.BIN.Equals(newClient.BIN)))
            {
                _dbContext.SaveChanges();
            }
            else
            {
                _dbContext.Client.Add(newClient);
                _dbContext.SaveChanges();
            }
        }

        readonly string [] BankTypeCodes = new string [] { "HA","HB","HD","HU","HC"};
        readonly string[] LLCTypeCodes = new string[] { "HA" };

        public ClientType GetClientType(string v)
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

        public Employee GetCurrentEmployee()
        {
            string login = Environment.UserName;
            if (_dbContext.Employees.Any(i => i.CodeName.ToLower().Equals(Environment.UserName.ToLower())))
            {
                return _dbContext.Employees.FirstOrDefault(i => i.CodeName.ToLower().Equals(login.ToLower()));
            }
            else
            {
                Employee newEmpl = new Employee();
                newEmpl.CodeName = login;
                newEmpl.Name = "Null";
                _dbContext.Employees.Add(newEmpl);
                _dbContext.SaveChanges();
                return newEmpl;
            }
        }

        public void AddScoring(Scoring newScoring)
        {
            _dbContext.Scorings.Add(newScoring);
            Commit();
        }

        public void SaveScoring(Scoring scoring)
        {
            Commit();
        }
    }
}
