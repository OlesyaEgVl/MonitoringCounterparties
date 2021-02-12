using System.ComponentModel.DataAnnotations.Schema;

namespace ContragentAnalyse.Model.Entities
{
    [Table("ScoringToCriteria")]
    public class ScoringToCriteria
    {
        [Column("scoring")]
        public int ScoringId { get; set; }
        [Column("criteria")]
        public int CriteriaId { get; set; }
        [ForeignKey(nameof(ScoringId))]
        public virtual Scoring Scoring { get; set; }
        [ForeignKey(nameof(CriteriaId))]
        public virtual Criteria Criteria { get; set; }
    }
}
