using System;
using System.Collections.Generic;
using System.Text;
using ContragentAnalyse.Model.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContragentAnalyse.Model.Entities
{
    public class StopFactors : BaseEntity
    {
        public DateTime Date { get; set; }
        public int Bank_Id { get; set; }
        public string Comment { get; set; }
        public string Measure { get; set; }
        [ForeignKey(nameof(Bank_Id))]
        public virtual Bank Bank { get; set; }
    }
}
