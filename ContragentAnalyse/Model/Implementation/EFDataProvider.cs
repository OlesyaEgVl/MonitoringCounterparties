﻿using ContragentAnalyse.Model.Entities;
using ContragentAnalyse.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContragentAnalyse.Model.Implementation
{
    public class EFDataProvider : IDataProvider
    {
        readonly DatabaseContext _dbContext;
        public EFDataProvider()
        {
            _dbContext = new DatabaseContext();
        }

        public IEnumerable<Client> GetClients(string BIN) => _dbContext.Client.
            Include(i=>i.TypeClient).
            Include(i => i.Actualization).
            Include(i => i.PrescoringScoring).
            Include(i=>i.Contracts).
            Include(i => i.RestrictedAccounts).
            Include(i => i.Contacts).
            Include(i=>i.Requests).
            Where(i => i.BIN.ToLower().Equals(BIN.ToLower()));

        public IEnumerable<Client> GetClientsByName(string Name) => _dbContext.Client.
            Include(i => i.TypeClient).
            Include(i => i.Actualization).
            Include(i => i.PrescoringScoring).
            Include(i => i.Contracts).
            Include(i => i.RestrictedAccounts).
            Include(i => i.Contacts).
            Include(i => i.Requests).
            Where(i => i.Name.ToLower().IndexOf(Name.ToLower()) > -1);
        public IEnumerable<Criteria> GetCriterias() => _dbContext.Criteria;
        public DateTime GetDateActual()
        {
            Actualization actual = _dbContext.Actualization.Include("Bank").FirstOrDefault(i => i.Id == 0); // Как соеденить таблицы и взять поле. которое нужно
            DateTime DateAct = actual.DateActEKS;
            return DateAct;
        }
        #region
        
        #endregion
        
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
        // GetLevelRisk - не используется
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

        public void AddClient(Client newClient)
        {
            throw new NotImplementedException();
        }
    }
}
