
using ContragentAnalyse.Model.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContragentAnalyse.Model.Entities
{
    public class ProductToScoring : BaseEntity
    {
        public int Product_Id { get; set; }
        public int Scoring_Id { get; set; }
        public bool? Inconsistent { get; set; }

        [ForeignKey(nameof(Product_Id))]
        public virtual BankProduct BankProduct { get; set; }

        [ForeignKey(nameof(Scoring_Id))]
        public virtual PrescoringScoring PrescoringScoring { get; set; }
    }
}
