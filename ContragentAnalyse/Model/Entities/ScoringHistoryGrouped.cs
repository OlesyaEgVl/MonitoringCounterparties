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
        public string CurrentHistoryComment
        {
            get
            {
                if (HistoryRecords==null)
                {
                    return string.Empty;
                }
                return HistoryRecords.Select(i => i.Comment).First();
            }
          
        }
        public bool ClosedClient { get; set; }
        public string Level
        {
            get
            {
                if (HistoryRecords == null)
                    return string.Empty;
                double riskLevel = 0f;
                riskLevel = HistoryRecords.Select(i => i.Criteria.Weight).Sum();
                string RiskLevelName = string.Empty;
                
                switch (riskLevel)
                {
                    case double n when n > 13.1:
                        RiskLevelName = $"{riskLevel.ToString("N1")} - Критичный";
                        break;
                    case double n when n >= 5.6 && n <= 13.1:
                        RiskLevelName = $"{riskLevel.ToString("N1")} - Высокий";
                        break;
                    case double n when n >= 3.5 && n <= 5.5:
                        RiskLevelName = $"{riskLevel.ToString("N1")} - Средний";
                        break;
                    case double n when n <= 3.4:
                        RiskLevelName = $"{riskLevel.ToString("N1")} - Низкий";
                        break;
                    default:
                        RiskLevelName = "Не определено";
                        break;
                }
                return RiskLevelName;
            }
        }
        public string NostroLevel
        {
            get
            {
                if (HistoryRecords == null)
                    return string.Empty;
                return HistoryRecords[0].NostroLevel;
            }
            set { }
        }
        /*public string NOSTRO { get; set; }
       
        public string LORO { get; set; }*/
       
        public string InappropriateBankProducts
        {
            get
            {
                return string.Empty; //TODO
            }
        }
    }
}
