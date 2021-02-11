using ContragentAnalyse.Model.Entities.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using LicenseContext = OfficeOpenXml.LicenseContext;
using OfficeOpenXml;

namespace ContragentAnalyse.Model.Entities
{
   public class Client : NamedEntity, INotifyPropertyChanged
    {
        public string Mnemonic { get; set; }
        public string ClientManager { get; set; }
        public bool? CardOP { get; set; }
        public bool?  SEB { get; set; }
        public bool? RestrictedAccount { get; set; }
        public int Client_type_Id { get; set; }
        public DateTime? BecomeClientDate { get; set; }
        public int ResponsibleUnit_Id { get; set; }
        public int CoordinatingEmployee_Id { get; set; }
        public string BIN { get; set; }
        public string AdditionalBIN { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string EnglName { get; set; }
        
        public string LicenceNumber { get; set; }
        public DateTime? LicenceEstDate { get; set; }
        public string RKC_BIK { get; set; }
        public string INN { get; set; }
        public string OGRN { get; set; }
        public DateTime? OGRN_Date { get; set; }
        public string RegName_RP { get; set; }
        public DateTime? RegDate_RP { get; set; }
        public string RegStruct_Name { get; set; }
        public bool? CurrencyLicence { get; set; }
        public string RegistrationRegion { get; set; }
        public string AddressPrime { get; set; }
        public DateTime? NextScoringDate { get; set; }
        public int? Country_Id { get; set; }
        public int? ClientToCurrency_Id { get; set; }
        public string ClientManagerNew { get; set; }
        public string Level
        {
            get
            {
                double riskLevel = 0;
                string RiskLevelName;
                if(Scorings.Count == 0)
                {
                    return "Не определено";
                }
                riskLevel = Scorings.Last().Criterias.Select(i => i.Criteria.Weight).Sum();
                RiskLevelName = riskLevel switch
                {
                    double n when n > 13.1 => $"{riskLevel.ToString("N1")} - Критичный",
                    double n when n >= 5.6 && n <= 13.1 => $"{riskLevel.ToString("N1")} - Высокий",
                    double n when n >= 3.5 && n <= 5.5 => $"{riskLevel.ToString("N1")} - Средний",
                    double n when n <= 3.4 => $"{riskLevel.ToString("N1")} - Низкий",
                    _ => "Не определено",
                };
                return RiskLevelName;
            }
            
        }
               
        public string BankProduct
        {
            get
            {
                if (Scorings.Count == 0)
                {
                    return "Не определено";
                }
                double riskLevel = Scorings.Last().Criterias.Select(i => i.Criteria.Weight).Sum();
                string BankProductName = string.Empty;
                BankProductName = riskLevel switch
                {
                    double n when n > 13.1 => "Сотрудничество приостановлено/запрещено",
                    double n when n >= 5.6 && n <= 13.1 => "Кор.счета рублевые ;Кор.счета валютные + Счет с ограничениями V ;Межбанк ; Синдицированное кредитование ;ALFA-FOREX и/или RISDA и/или ISDA и/или RISDA FI и/или RISDA онлайн и/или CSA онлайн;ALFA-FOREX и/или RISDA и/или ISDA и/или RISDA FI и/или RISDA онлайн и/или CSA онлайн и/или ISMA и/или Соглашения по ценным бумагам;ISMA и/или Соглашения по ценным бумагам;Драгметаллы;Организация секьюритизаций;Объединение банкоматных сетей;Кор.счета рублевые + Пластиковые карты и/или Договор по операциям ПК и/или Процессинг и/или Договор НПС (нац.платеж.сист.);",
                    double n when n >= 3.5 && n <= 5.5 => "Зарплатные проекты;Электр.банк.гарантия и/или Непокрыт.аккредитив.Бенефициар;Банкнотные сделки;Собственные векселя;Договор на инкассацию; ",
                    double n when n <= 3.4 && n > 0 => "Кор.счета валютные;Кор.счета валютные + Пластиковые карты и/или Договор по операциям ПК и/или Процессинг и/или Договор НПС (нац.платеж.сист.);Брокерское обслуживание;Депозитарное обслуживание;",
                    _ => "Не определено",
                };
                return BankProductName;
            }
        }

        public DateTime? ActualizationDate
        {
            get
            {
                if(Actualization != null && Actualization.Count > 0)
                {
                    return Actualization?.Max(i => i.DateActEKS);
                }
                else
                {
                    return null;
                }
            }
        }

        public string ActualizationStatus
        {
            get
            {
                if (Actualization != null && Actualization.Count > 0)
                {
                    return Actualization?.Max(i => i.Status);
                }
                else
                {
                    return null;
                }
            }
        }

        [ForeignKey(nameof(ResponsibleUnit_Id))]
        public virtual ResponsibleUnit ResponsibleUnit { get; set; }
        [ForeignKey(nameof(Client_type_Id))]
        public virtual ClientType TypeClient { get; set; }
        [ForeignKey(nameof(CoordinatingEmployee_Id))]
        public virtual Employee Employees { get; set; }
        [ForeignKey(nameof(Country_Id))]
        public virtual Country Country { get; set; }
        public List<ClientToCurrency> ClientToCurrency { get; set; } = new List<ClientToCurrency>();
        public ObservableCollection<Request> Requests { get; set; } = new ObservableCollection<Request>();
        public ObservableCollection<Actualization> Actualization { get; set; } = new ObservableCollection<Actualization>();
        public ObservableCollection<Scoring> Scorings { get; set; } = new ObservableCollection<Scoring>();
        public ObservableCollection<StopFactors> StopFactors { get; set; } = new ObservableCollection<StopFactors>();
        public ObservableCollection<RestrictedAccounts> RestrictedAccounts { get; set; } = new ObservableCollection<RestrictedAccounts>();
        public ObservableCollection<Contacts> Contacts { get; set; } = new ObservableCollection<Contacts>();
        public ObservableCollection<ClientToContracts> ClientToContracts { get; set; } = new ObservableCollection<ClientToContracts>();

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
