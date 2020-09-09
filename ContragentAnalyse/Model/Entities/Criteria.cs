using System;
using System.Collections.Generic;
using System.Text;
using ContragentAnalyse.Model.Entities.Base;

namespace ContragentAnalyse.Model.Entities
{
    public class Criteria: NamedEntity
    {
        public float Weight { get; set; }
        public List<CriteriaToScoring> CriteriaToScoring { get; set; }
    }
}
