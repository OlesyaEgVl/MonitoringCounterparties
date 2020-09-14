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
        public int Bank_Id { get; set; }
        public double? NO_Score { get; set; }
        public double? Nostro_Score { get; set; }
        public DateTime? DateNextScoring { get; set; }

        [ForeignKey(nameof(Bank_Id))]
        public virtual Bank Bank { get; set; }
        [ForeignKey(nameof(ScoringType_Id))]
        public virtual ScoringType ScoringType { get; set; }
        public List <CriteriaToScoring> CriteriaToScoring { get; set; }
    }
}
