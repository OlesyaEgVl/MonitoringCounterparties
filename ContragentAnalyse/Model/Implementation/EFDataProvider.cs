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
            Include(i => i.Contracts).
            Include(i => i.RestrictedAccounts).
            Include(i => i.Contacts).
            Include(i => i.Requests).
            Include(i => i.ClientToCurrency).
                ThenInclude(i => i.Currency).
            Where(i => i.BIN.ToLower().Equals(BIN.ToLower()));

        public IEnumerable<Client> GetClientsByName(string Name) => _dbContext.Client.
            Include(i => i.TypeClient).
            Include(i => i.Actualization).
            Include(i => i.PrescoringScoring).
            Include(i => i.Contracts).
            Include(i => i.RestrictedAccounts).
            Include(i => i.Contacts).
            Include(i => i.Requests).
            Include(i=>i.ClientToCurrency).
                ThenInclude(i=>i.Currency).
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

        public IEnumerable<ContactType> GetContactTypes()
        {
            return _dbContext.ContactType;
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
            
            /*string connectionString = @"Server=A105512\\A105512;Database=CounterpartyMonitoring;Integrated Security=false;Trusted_Connection=True;MultipleActiveResultSets=True;User Id = CounterPartyMonitoring_user; Password = orppaAdmin123!";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.CommandText = @"INSERT INTO Client VALUES (@ClientManager, @Client_type_Id ,@BecomeClientDate,@AdditionalBIN,@ShortName,@FullName,@EnglName  ,@LicenceNumber,@LicenceEstDate,@RKC_BIK,@INN,@OGRN,@OGRN_Date,@RegName_RP,@RegDate_RP,@CurrencyLicence ,@RegistrationRegion,@NextScoringDate,@Country_Id,@Currency_Id)";
                command.Parameters.Add("@ClientManager", SqlDbType.NVarChar);
                command.Parameters.Add("@Client_type_Id", SqlDbType.Int);
                command.Parameters.Add("@BecomeClientDate", SqlDbType.Date);
                command.Parameters.Add("@AdditionalBIN", SqlDbType.NVarChar);
                command.Parameters.Add("@ShortName", SqlDbType.NVarChar);
                command.Parameters.Add("@FullName", SqlDbType.NVarChar);
                command.Parameters.Add("@EnglName", SqlDbType.NVarChar);
                command.Parameters.Add("@LicenceNumber", SqlDbType.NVarChar);
                command.Parameters.Add("@LicenceEstDate", SqlDbType.Date);
                command.Parameters.Add("@RKC_BIK", SqlDbType.NVarChar);
                command.Parameters.Add("@INN", SqlDbType.NVarChar);
                command.Parameters.Add("@OGRN", SqlDbType.NVarChar);
                command.Parameters.Add("@OGRN_Date", SqlDbType.Date);
                command.Parameters.Add("@RegName_RP", SqlDbType.NVarChar);
                command.Parameters.Add("@RegDate_RP", SqlDbType.Date);
                command.Parameters.Add("@CurrencyLicence", SqlDbType.Bit);
                command.Parameters.Add("@RegistrationRegion", SqlDbType.NVarChar);
                command.Parameters.Add("@NextScoringDate", SqlDbType.Date);
                command.Parameters.Add("@Country_Id", SqlDbType.Int);
                command.Parameters.Add("@Currency_Id", SqlDbType.Int);
                
                // массив для хранения бинарных данных файла
                byte[] imageData;
                using (System.IO.FileStream fs = new System.IO.FileStream(connectionString, FileMode.Open))
                {
                    imageData = new byte[fs.Length];
                    fs.Read(imageData, 0, imageData.Length);
                }
                // передаем данные в команду через параметры
                command.Parameters["@ClientManager"].Value = client.ClientManager;
                command.Parameters["@Client_type_Id"].Value = client.Client_type_Id;
                command.Parameters["@BecomeClientDate"].Value = client.BecomeClientDate;
                command.Parameters["@AdditionalBIN"].Value = client.AdditionalBIN;
                command.Parameters["@ShortName"].Value = client.ShortName;
                command.Parameters["@FullName"].Value = client.FullName;
                command.Parameters["@EnglName"].Value = client.EnglName;
                command.Parameters["@LicenceNumber"].Value = client.LicenceNumber;
                command.Parameters["@LicenceEstDate"].Value = client.LicenceEstDate;
                command.Parameters["@RKC_BIK"].Value = client.RKC_BIK;
                command.Parameters["@INN"].Value = client.INN;
                command.Parameters["@OGRN"].Value = client.OGRN;
                command.Parameters["@OGRN_Date"].Value = client.OGRN_Date;
                command.Parameters["@RegName_RP"].Value = client.RegName_RP;
                command.Parameters["@RegDate_RP"].Value = client.RegDate_RP;
                command.Parameters["@CurrencyLicence"].Value = client.CurrencyLicence;
                command.Parameters["@RegistrationRegion"].Value = client.RegistrationRegion;
                command.Parameters["@NextScoringDate"].Value = client.NextScoringDate;
                command.Parameters["@Country_Id"].Value = client.Country_Id;
                command.Parameters["@Currency_Id"].Value = client.Currency_Id;


                command.ExecuteNonQuery();
                // throw new NotImplementedException();
            }*/
            
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

        public string AddCriteriaList(Criteria[] criteriaslist)
        {
           string result="";
           string bankproduct;
           double sum=0;
           foreach (Criteria crit in criteriaslist)
            {
                sum += crit.Weight;
            }
            if (sum >= 13.1)
            {
                result= sum + " - Критичный";
                bankproduct = "Сотрудничество приостановлено/запрещено";

            }
            else if ((sum >= 5.6) && (sum <= 13))
            {
                result = sum + " - Высокий";
                bankproduct = "Открытие корреспондентских счетов в рублях; в иностранной валюте - только для внутренних расчетов и/или собственных операций;Привлечение / размещение межбанковских кредитов/ депозитов;Синдицированное кредитование;Безналичные конверсионные операции;Операции с производными финансовыми инструментами;Сделки с ценными бумагами;Сделки купли-продажи драгоценных металлов;Организация секьюритизаций;Объединение банкоматных сетей;Расчеты в рублях через международные платежные системы;";
            }
            else if ((sum >= 3.5) && (sum <= 5.5))
            {
                result = sum + " - Средний";
                bankproduct = "Банковские продукты для клиентов с высоким уровнем комплаенс-риска + Зарплатные проекты;Документарные операции(аккредитивы, инкассо, гарантии);Банкнотные сделки;Сделки с векселями;Инкассация";
            }
            else if (sum <= 3.5)
            {
                result = sum + " - Низкий";
                bankproduct = "Банковские продукты для клиентов со средним уровнем комплаенс-риска + Открытие корреспондентских счетов без ограничения по валюте счета и режиму счета;Трансграничные расчеты и / или расчеты в иностранной валюте через международные платежные системы;Брокерское обслуживание;Депозитарное обслуживание;";
            }
            return result;  
        }

       

       
    }
}
