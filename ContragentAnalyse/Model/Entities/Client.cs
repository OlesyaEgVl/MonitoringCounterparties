using ContragentAnalyse.Model.Entities.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ContragentAnalyse.Model.Entities
{
   public class Client : NamedEntity
    {
        public string Mnemonic { get; set; }
        public string ClientManager { get; set; }
        public bool? CardOP { get; set; }
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
        public string Level
        {
            get
            {
                float riskLevel = 0;
                string RiskLevelName = "Не определено";
                if (ClientToCriteria == null)
                {
                    RiskLevelName = "Не определено";
                    return RiskLevelName;
                }
                else
                {
                    riskLevel = ClientToCriteria.Select(i => i.Criteria.Weight).Sum(); // ругается, что бывает нулевое значение
                    RiskLevelName = string.Empty;
                    switch (riskLevel)
                    {
                        case float n when n > 13.1:
                            RiskLevelName = $"{riskLevel} - Критичный";
                            break;
                        case float n when n >= 5.6 && n <= 13.1:
                            RiskLevelName = $"{riskLevel} - Высокий";
                            break;
                        case float n when n >= 3.5 && n <= 5.5:
                            RiskLevelName = $"{riskLevel} - Средний";
                            break;
                        case float n when n <= 3.4:
                            RiskLevelName = $"{riskLevel} - Низкий";
                            break;
                        default:
                            RiskLevelName = "Не определено";
                            break;
                    }
                    return RiskLevelName;
                }
            }
        }
               
        public string BankProduct
        {
            get
            {
                float riskLevel = ClientToCriteria.Select(i => i.Criteria.Weight).Sum();
                string BankProductName = string.Empty;
                switch (riskLevel)
                {
                    case float n when n > 13.1:
                        BankProductName = "Сотрудничество приостановлено/запрещено";
                        break;
                    case float n when n >= 5.6 && n <= 13.1:
                        BankProductName = "Открытие корреспондентских счетов в рублях; " + "\n" + " в иностранной валюте - только для внутренних расчетов и/или собственных операций; " + "\n" + "Привлечение / размещение межбанковских кредитов/ депозитов;Синдицированное кредитование;Безналичные конверсионные операции;Операции с производными финансовыми инструментами;Сделки с ценными бумагами; " + "\n" + "Сделки купли-продажи драгоценных металлов;Организация секьюритизаций;Объединение банкоматных сетей;" + "\n" + "Расчеты в рублях через международные платежные системы;";
                        break;
                    case float n when n >= 3.5 && n <= 5.5:
                        BankProductName = "Банковские продукты для клиентов с высоким уровнем комплаенс-риска + " + "\n" + "Зарплатные проекты;" + "\n"+"Документарные операции(аккредитивы, инкассо, гарантии);Банкнотные сделки;Сделки с векселями;Инкассация;";
                        break;
                    case float n when n <= 3.4 && n>0:
                        BankProductName = "Банковские продукты для клиентов со средним уровнем комплаенс-риска + " + "\n" + "Открытие корреспондентских счетов без ограничения по валюте счета и режиму счета;Трансграничные расчеты и / или расчеты в иностранной валюте через международные платежные системы; " + "\n" + "Брокерское обслуживание;Депозитарное обслуживание;";
                        break;
                    default:
                        BankProductName = "Не определено";
                        break;
                }
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
        [ForeignKey(nameof(ResponsibleUnit_Id))]
        public virtual ResponsibleUnit ResponsibleUnit { get; set; }
        [ForeignKey(nameof(Client_type_Id))]
        public virtual TypeClient TypeClient { get; set; }
        [ForeignKey(nameof(CoordinatingEmployee_Id))]
        public virtual Employees Employees { get; set; }
        [ForeignKey(nameof(Country_Id))]
        public virtual Country Country { get; set; }
        public List<ClientToCurrency> ClientToCurrency { get; set; }

        private ObservableCollection<Request> requests = new ObservableCollection<Request>();
        public ObservableCollection<Request> Requests
        {
            get => requests;
            set => requests = value;
        }
        public List<Actualization> Actualization { get; set; }
        public List<PrescoringScoring> PrescoringScoring { get; set; }
        public List<PrescoringScoringHistory> PrescoringScoringHistory { get; set; }
        public List<StopFactors> StopFactors { get; set; }
        public List<RestrictedAccounts> RestrictedAccounts { get; set; }
        public List<Contacts> Contacts { get; set; }
        public List<ClientToCriteria> ClientToCriteria { get; set; }
        public List<ClientToContracts> ClientToContracts { get; set; }

    }
}
