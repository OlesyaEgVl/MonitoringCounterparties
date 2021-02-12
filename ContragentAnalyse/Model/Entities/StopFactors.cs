using ContragentAnalyse.Model.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContragentAnalyse.Model.Entities
{
    public class StopFactors : BaseEntity
    {
        public DateTime Date { get; set; }
        public int Client_Id { get; set; }
        public string Comment { get; set; }
        public string Measure { get; set; }
        [ForeignKey(nameof(Client_Id))]
        public virtual Client Client { get; set; }
    }
}
