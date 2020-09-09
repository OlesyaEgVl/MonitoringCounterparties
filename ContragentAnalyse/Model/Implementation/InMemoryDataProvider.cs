using ContragentAnalyse.Model.Entities;
using ContragentAnalyse.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;


namespace ContragentAnalyse.Model.Implementation
{
    public class InMemoryDataProvider : IDataProvider
    {
        public Entities.Bank GetBank(string BIN)
        {
            Entities.Bank banks = new Entities.Bank();
            return banks;
        }

        public DateTime GetDateActual()
        {
            return DateTime.Today;
        }
        public DateTime? GetDateDirection()
        {
            return DateTime.Today;
        }
        public DateTime? GetDateReceive()
        {
            return DateTime.Today;
        }
        public string GetComments()
        {
            return string.Empty;
        }
        public DateTime? GetDateNextScoring()
        {
            return DateTime.Today;
        }
        public string GetLevelRisk() 
        { return string.Empty; }
        public string GetClientManager()
        { return string.Empty; }
        public string GetCriteria()
        { return string.Empty; }
        public string GetContracts()
        {
            return string.Empty;
        }
        public bool? GetCardOP()
        {

            return true;
        }
        public string GetAccountNumber()
        {
            return string.Empty;
        }
        public string GetContacts()
        {
            return string.Empty;
        }

        public List<Client> GetClients(string BIN)
        {
            throw new NotImplementedException();
        }
    }
}
