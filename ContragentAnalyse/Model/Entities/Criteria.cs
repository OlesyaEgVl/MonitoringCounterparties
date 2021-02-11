using ContragentAnalyse.Model.Entities.Base;
using System.Collections.Generic;

namespace ContragentAnalyse.Model.Entities
{
    public class Criteria: NamedEntity
    {
        public double Weight { get; set; }
        public List<ScoringToCriteria> Scorings { get; set; } = new List<ScoringToCriteria>();
    }
}
