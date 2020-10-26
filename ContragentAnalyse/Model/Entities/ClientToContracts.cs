using ContragentAnalyse.Model.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ContragentAnalyse.Model.Entities
{
    public class ClientToContracts : BaseEntity
    {
        public int Contracts_Id { get; set; } 
        public int Client_Id { get; set; } 
        [ForeignKey(nameof(Client_Id))]
        public virtual Client Client { get; set; }
        [ForeignKey(nameof(Contracts_Id))]
        public Contracts Contracts { get; set; }
    }
}
