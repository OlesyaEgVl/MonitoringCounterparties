using ContragentAnalyse.Model.Entities;
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
        DatabaseContext _dbContext;
        public EFDataProvider()
        {
            _dbContext = new DatabaseContext();
        }

        public Bank GetBank(string BIN) => _dbContext.Banks.FirstOrDefault(i => i.BIN.ToLower().Equals(BIN.ToLower()));

        public IEnumerable<Client> GetClients(string BIN) => _dbContext.Client.
            Include(i=>i.Bank).
            Include(i=>i.TypeClient).
            Include(i=>i.Bank.Actualizations).
            Include(i => i.Bank.PrescoringScoring).
            Include(i => i.Bank).
                ThenInclude(i => i.PrescoringScoring).
                ThenInclude(i => i.CriteriaToScoring).
            Include(i => i.Bank.Client).
            Include(i=>i.Bank.Contracts).
            Include(i => i.Bank.RestrictedAccounts).
            Include(i => i.Bank.Contacts).
            Include(i=>i.Requests).
            Where(i => i.Bank.BIN.ToLower().Equals(BIN.ToLower()));

        public IEnumerable<Client> GetClientsName(string Name) => _dbContext.Client.
            Include(i => i.Bank).
            Include(i => i.TypeClient).
            Include(i => i.Bank.Actualizations).
            Include(i => i.Bank.PrescoringScoring).
            Include(i=> i.Bank).
                ThenInclude(i=>i.PrescoringScoring).
                ThenInclude(i=>i.CriteriaToScoring).
            Include(i => i.Bank.Client).
            Include(i => i.Bank.Contracts).
            Include(i => i.Bank.RestrictedAccounts).
            Include(i => i.Bank.Contacts).
            Include(i => i.Requests).
            Where(i => i.Bank.Name.ToLower().IndexOf(Name.ToLower()) > -1);
        
        public DateTime GetDateActual()
        {
            Actualization actual = _dbContext.Actualization.Include("Bank").FirstOrDefault(i => i.Id == 0); // Как соеденить таблицы и взять поле. которое нужно
            DateTime DateAct = actual.DateActEKS;
            return DateAct;
        }

        public DateTime? GetDateNextScoring()
        {
            PrescoringScoring nextdate = _dbContext.PrescoringScoring.Include("Bank").FirstOrDefault(i => i.Id == 0); // Как соеденить таблицы и взять поле. которое нужно
            DateTime? DateNext = nextdate.DateNextScoring;
            return DateNext;
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
    }
}
