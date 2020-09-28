using ContragentAnalyse.Model.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ContragentAnalyse.Model.Entities
{
   public class Client : NamedEntity
    {
        public string Mnemonic { get; set; }
        public string ClientManager { get; set; }
        public bool? CardOP { get; set; }
        public int Client_type_Id { get; set; }
        public DateTime BecomeClientDate { get; set; }
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
        public int Country_Id { get; set; }
        public string Level
        {
            get
            {
                return "Здесь будет уровень риска";
            }
        }

        /*public DateTime? NextScoringDate
        {
            get
            {
                return PrescoringScoring.Max(i => i.DateNextScoring);
            }
        }
        */
       
        [ForeignKey(nameof(ResponsibleUnit_Id))]
        public virtual ResponsibleUnit ResponsibleUnit { get; set; }
        [ForeignKey(nameof(Client_type_Id))]
        public virtual TypeClient TypeClient { get; set; }
        [ForeignKey(nameof(CoordinatingEmployee_Id))]

        public virtual Employees Employees { get; set; }
        [ForeignKey(nameof(Country_Id))]
        public virtual Country Country { get; set; }
        private List<Request> requests = new List<Request>();
        public List<Request> Requests { get => requests; set => requests = value; }
        public List<Actualization> Actualization { get; set; }
        public List<PrescoringScoring> PrescoringScoring { get; set; }
        public List<Contracts> Contracts { get; set; }
        public List<StopFactors> StopFactors { get; set; }
        public List<RestrictedAccounts> RestrictedAccounts { get; set; }
        public List<Contacts> Contacts { get; set; }
        public List<ClientToCrineria> ClientToCrineria { get; set; }

    }
}
