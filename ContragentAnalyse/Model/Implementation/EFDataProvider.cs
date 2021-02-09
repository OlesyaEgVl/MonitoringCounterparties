﻿using ContragentAnalyse.Model.Entities;
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
           // Include(i => i.PrescoringScoring).
            Include(i => i.PrescoringScoringHistory).
                ThenInclude(i => i.Employees).
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

        public IEnumerable<Client> GetClients() =>
            _dbContext.Client.
            Include(i => i.TypeClient).
            Include(i => i.Actualization).
            //Include(i => i.PrescoringScoring).
            Include(i => i.PrescoringScoringHistory).
                ThenInclude(i => i.Employees).
            Include(i => i.RestrictedAccounts).
            Include(i => i.Contacts).
            Include(i => i.Country).
            Include(i => i.Requests).
            Include(i => i.ClientToCurrency).
                ThenInclude(i => i.Currency).
            Include(i => i.ClientToCriteria).
                ThenInclude(i => i.Criteria).
            Include(i => i.ClientToContracts).
                ThenInclude(i => i.Contracts);

        public IEnumerable<Client> GetClientsByName(string Name) => _dbContext.Client.
            Include(i => i.TypeClient).
            Include(i => i.Actualization).
           // Include(i => i.PrescoringScoring).
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
            Where(i => i.FullName.ToLower().IndexOf(Name.ToLower()) > -1);
        public IEnumerable<Criteria> GetCriterias() => _dbContext.Criteria;
        public IEnumerable<PrescoringScoringHistory> GetScoringHistory() => _dbContext.PrescoringScoringHistory;
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

        //Удалить старые критерии и добавить новые
        public void UpdateRiskLevel(int ClientId, ScoringHistoryGrouped selectedCriterias, DateTime preScoringDate)
        {
            _dbContext.PrescoringScoringHistory.RemoveRange(_dbContext.PrescoringScoringHistory.Where(i => i.Client_Id == ClientId && i.DatePresScor == preScoringDate));

            foreach(PrescoringScoringHistory newScoring in selectedCriterias.HistoryRecords)
            {
                newScoring.Id = 0;
                _dbContext.PrescoringScoringHistory.Add(newScoring);
                _dbContext.Entry<PrescoringScoringHistory>(newScoring).State = EntityState.Added;
            }
            
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
                // MessageBox.Show("Клиент уже существует!");
               // _dbContext.Client.Add(newClient.Name);
                _dbContext.SaveChanges();
            }
            else
            {
                _dbContext.Client.Add(newClient);
                _dbContext.SaveChanges();
               // MessageBox.Show("Клиент добавлен!");
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
                newEmpl.Name = "Null";
                _dbContext.Employees.Add(newEmpl);
                _dbContext.SaveChanges();
                return newEmpl;
            }
        }

        public IEnumerable<ScoringHistoryGrouped> GetClientHistory(Client client)
        {
            List<ScoringHistoryGrouped> output = new List<ScoringHistoryGrouped>();
            List<PrescoringScoringHistory> historyRecords = _dbContext.PrescoringScoringHistory.Where(i => i.Client_Id == client.Id).ToList();
            foreach (PrescoringScoringHistory rec in historyRecords)
            {
                if (output.Any(i => DateTime.Equals(i.HistoryDate, rec.DatePresScor)))
                {
                    output.First(i => DateTime.Equals(i.HistoryDate, rec.DatePresScor)).HistoryRecords.Add(rec);
                }
                else
                { 
                    ScoringHistoryGrouped newValue = new ScoringHistoryGrouped
                    {
                        HistoryDate = rec.DatePresScor,
                        HistoryRecords = new List<PrescoringScoringHistory>(),
                        EmployeeName = rec.Employees.Name
                        
                    };
                    newValue.HistoryRecords.Add(rec);
                    output.Add(newValue);
                }
            }
            return output;
        }

        public void AddScoring(IEnumerable<PrescoringScoringHistory> records)
        {
            foreach(PrescoringScoringHistory scoring in records)
            {
                _dbContext.PrescoringScoringHistory.Add(scoring);
            }
            _dbContext.SaveChanges();
        }
    }
}
