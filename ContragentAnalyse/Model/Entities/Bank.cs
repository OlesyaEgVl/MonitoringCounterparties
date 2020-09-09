using ContragentAnalyse.Model.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContragentAnalyse.Model.Entities
{
    [Table("Banks")]
    public class Bank : NamedEntity
    {
        public string BIN { get; set; }
        public string AdditionalBIN { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string EnglName { get; set; }
        public string Country{ get; set; }
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
        public List<Actualization> Actualizations { get; set; }
        public List<PrescoringScoring> PrescoringScoring { get; set; }
        public List<Client> Client { get; set; }
        public List<Contracts> Contracts { get; set; }
        public List<RestrictedAccounts> RestrictedAccounts { get; set; }
        public List<Contacts> Contacts { get; set; }
    }
}
