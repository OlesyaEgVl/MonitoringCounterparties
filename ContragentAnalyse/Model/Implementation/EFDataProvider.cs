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
            Include(i => i.Actualizations).
            Include(i => i.PrescoringScoring).
            Include(i=>i.Contracts).
            Include(i => i.RestrictedAccounts).
            Include(i => i.Contacts).
            Include(i=>i.Requests).
            Where(i => i.BIN.ToLower().Equals(BIN.ToLower()));

        public IEnumerable<Client> GetClientsByName(string Name) => _dbContext.Client.
            Include(i => i.TypeClient).
            Include(i => i.Actualizations).
            Include(i => i.PrescoringScoring).
            Include(i => i.Contracts).
            Include(i => i.RestrictedAccounts).
            Include(i => i.Contacts).
            Include(i => i.Requests).
            Where(i => i.Name.ToLower().IndexOf(Name.ToLower()) > -1);
        
        public DateTime GetDateActual()
        {
            Actualization actual = _dbContext.Actualization.Include("Bank").FirstOrDefault(i => i.Id == 0); // Как соеденить таблицы и взять поле. которое нужно
            DateTime DateAct = actual.DateActEKS;
            return DateAct;
        }
        #region
        //подсказки в поле поиска Риск
        /// </summary>
        /// <param name="process">Процесс, к которому нужно подобратьподсказки</param>
       /// <returns>Стек тем, введенных ранее пользователем</returns>
        /* public Stack<string> GetSubjectHints(Process process)
         {
            Stack<string> subjects = new Stack<string>();
            Dictionary<string, int> subjectCounted = new Dictionary<string, int>();

             int proc_id;

            if (process != null)
            {
                 proc_id = process.Id;
 
                  foreach(string item in Criteria.Name.Where(i=>i.Name.ToLower()) &&
               i.Subject.Length > 0 && i.Process_id == proc_id).Select(i => i.Subject).ToArray())
                {
                   if (subjectCounted.ContainsKey(item))
                    {
                        subjectCounted[item]++;
                      }
                     else
                     {
                        subjectCounted.Add(item, 1);
                     }
                 }
                 foreach(KeyValuePair<string, int> item in (from i in subjectCounted orderby i.Value ascending select i))
                {
                      subjects.Push(item.Key);
               }
             }
             else
            {
                 foreach (string item in context.TimeSheetTableSet.Where(i =>
 i.Analytic.UserName.ToLower().Equals(Environment.UserName.ToLower()) &&
                  i.Subject.Length > 0).Select(i => i.Subject).ToArray())
                  {
                    if (subjectCounted.ContainsKey(item))
                     {
                        subjectCounted[item]++;
                     }
                     else
                     {
                      subjectCounted.Add(item, 1);
                      }
                }
                foreach (KeyValuePair<string, int> item in (from i in
  subjectCounted orderby i.Value ascending select i))
                 {
                     subjects.Push(item.Key);
                }
            }
              return subjects;
          }

        */
        #endregion
        /*public DateTime? GetDateNextScoring()
        {
            PrescoringScoring nextdate = _dbContext.PrescoringScoring.Include("Bank").FirstOrDefault(i => i.Id == 0); // Как соеденить таблицы и взять поле. которое нужно
            DateTime? DateNext = nextdate.DateNextScoring;
            return DateNext;
        }*/
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
