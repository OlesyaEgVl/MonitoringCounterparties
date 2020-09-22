using ContragentAnalyse.Model.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ContragentAnalyse.Model.Entities
{
    public class ClientToCrineria : BaseEntity
    {
        public int Criteria_Id { get; set; }
        public int Client_Id { get; set; }
        [ForeignKey(nameof(Criteria_Id))]
        public virtual Criteria Criteria { get; set; }
        [ForeignKey(nameof(Client_Id))]
        public virtual Client Client { get; set; }


    }
}
