using ContragentAnalyse.Model.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContragentAnalyse.Model.Entities
{
    public class Actualization : BaseEntity
    {
        public int Client_Id { get; set; }
        public int Status_Id { get; set; }
        public DateTime DateEKS{ get; set; }
        public DateTime DateActEKS { get; set; }
        public int Type_Agreement_Id { get; set; }
        [ForeignKey(nameof(Client_Id))]
        public virtual Client Client { get; set; }
        [ForeignKey(nameof(Status_Id))]
        public virtual Status Status { get; set; }
        [ForeignKey(nameof(Type_Agreement_Id))]
        public virtual TypeAgreement TypeAgreement { get; set; }
    }
}
