using ContragentAnalyse.Model.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContragentAnalyse.Model.Entities
{
    public class PrescoringScoring : BaseEntity
    {
        public DateTime DatePresScor { get; set; }
        public int ScoringType_Id { get; set; }
        public int Client_Id { get; set; }
        [ForeignKey(nameof(Client_Id))]
        public virtual Client Client { get; set; }
        [ForeignKey(nameof(ScoringType_Id))]
        public virtual ScoringType ScoringType { get; set; }
        
    }
}
