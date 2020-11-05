using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContragentAnalyse.Model.Entities
{
    public class ScoringHistoryGrouped
    {
        public List<PrescoringScoringHistory> HistoryRecords { get; set; }
        public DateTime HistoryDate { get; set; }
        public string EmployeeName { get; set; }
        public bool ClosedClient { get; set; }
        public string Level
        {
            get
            {
                if (HistoryRecords == null)
                    return string.Empty;
                float riskLevel = 0f;
                riskLevel = HistoryRecords.Select(i => i.Criteria.Weight).Sum();
                string RiskLevelName = string.Empty;
                switch (riskLevel)
                {
                    case float n when n > 13.1:
                        RiskLevelName = $"{riskLevel} - Критичный";
                        break;
                    case float n when n >= 5.6 && n <= 13.1:
                        RiskLevelName = $"{riskLevel} - Высокий";
                        break;
                    case float n when n >= 3.5 && n <= 5.5:
                        RiskLevelName = $"{riskLevel} - Средний";
                        break;
                    case float n when n <= 3.4:
                        RiskLevelName = $"{riskLevel} - Низкий";
                        break;
                    default:
                        RiskLevelName = "Не определено";
                        break;
                }
                return RiskLevelName;
            }
        }
        public string InappropriateBankProducts
        {
            get
            {
                return string.Empty; //TODO
            }
        }
    }
}
