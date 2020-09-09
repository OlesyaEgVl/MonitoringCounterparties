using ContragentAnalyse.Model.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContragentAnalyse.Model.Entities
{
   public class Client : BaseEntity
    {
        public int? Bank_Id { get; set; }
        public string Mnemonic { get; set; }
        public string ClientManager { get; set; }
        public bool? CardOP { get; set; }
        public int Client_type_Id { get; set; }
        public DateTime BecomeClientDate { get; set; }
        public int ResponsibleUnit_Id { get; set; }
        public int CoordinatingEmployee_Id { get; set; }
        [ForeignKey(nameof(Bank_Id))]
        public virtual Bank Bank { get; set; }
        [ForeignKey(nameof(ResponsibleUnit_Id))]
        public virtual ResponsibleUnit ResponsibleUnit { get; set; }
        [ForeignKey(nameof(Client_type_Id))]
        public virtual TypeClient TypeClient { get; set; }
        [ForeignKey(nameof(CoordinatingEmployee_Id))]
        public virtual Employees Employees { get; set; }
    }
}
