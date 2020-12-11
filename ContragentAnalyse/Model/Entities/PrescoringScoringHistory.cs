using ContragentAnalyse.Model.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContragentAnalyse.Model.Entities
{
    public class PrescoringScoringHistory : BaseEntity
    {
        public DateTime DatePresScor { get; set; }
        public int Employee_Id { get; set; }
        public int Criteria_Id { get; set; }
        public int Client_Id { get; set; }
        public bool ClosedClient { get; set; }
        public string Comment { get; set; }
        public string NOSTRO { get; set; }
        public string LORO { get; set; }

        [ForeignKey(nameof(Client_Id))]
        public virtual Client Client { get; set; }
        [ForeignKey(nameof(Employee_Id))]
        public virtual Employees Employees { get; set; }
        [ForeignKey(nameof(Criteria_Id))]
        public virtual Criteria Criteria{ get; set; }
        public string NostroLevel { get; set; }
    }
}
