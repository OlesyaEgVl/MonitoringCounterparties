using ContragentAnalyse.Model.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ContragentAnalyse.Model.Entities
{
    public class ClientToCriteria : BaseEntity
    {
        public int Criteria_Id { get; set; }
        public int Client_Id { get; set; }
        public DateTime? DateAdd { get; set; }
        // public int Employee_Id { get; set; }
        // public DateTime? DateAddCriteria { get; set; }
        [ForeignKey(nameof(Criteria_Id))]
        public virtual Criteria Criteria { get; set; }
        [ForeignKey(nameof(Client_Id))]
        public virtual Client Client { get; set; }
      /*  [ForeignKey(nameof(Employee_Id))]
        public virtual Employees Employees { get; set; }
      */


    }
}
