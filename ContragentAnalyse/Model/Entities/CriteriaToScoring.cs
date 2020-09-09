using System;
using System.Collections.Generic;
using System.Text;
using ContragentAnalyse.Model.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContragentAnalyse.Model.Entities
{
    public class CriteriaToScoring : BaseEntity
    {
        public int? Criterion_Id { get; set; }
        public int PrescoringScoring_Id { get; set; }

        [ForeignKey(nameof(Criterion_Id))]
        public virtual Criteria Criteria { get; set; }

        [ForeignKey(nameof(PrescoringScoring_Id))]
        public virtual PrescoringScoring PrescoringScoring { get; set; }

    }
}
