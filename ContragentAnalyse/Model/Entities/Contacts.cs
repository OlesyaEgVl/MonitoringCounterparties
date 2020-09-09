using ContragentAnalyse.Model.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContragentAnalyse.Model.Entities
{
    public class Contacts : BaseEntity
    {
        public int ContactType_Id { get; set; }
        public string Value { get; set; }
        public string ContactFIO { get; set; }
        public int Bank_Id { get; set; }

        [ForeignKey(nameof(Bank_Id))]
        public virtual Bank Bank { get; set; }

        [ForeignKey(nameof(ContactType_Id))]
        public virtual ContactType ContactType { get; set; }
    }
}
