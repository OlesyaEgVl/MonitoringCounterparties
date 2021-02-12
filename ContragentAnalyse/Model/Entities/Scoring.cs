using ContragentAnalyse.Model.Entities.Base;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ContragentAnalyse.Model.Entities
{
    [Table("Scoring")]
    public class Scoring : BaseEntity, INotifyPropertyChanged
    {
        public Scoring()
        {
            Criterias.CollectionChanged += Criterias_CollectionChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Criterias_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(TotalRiskLevel));
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("employee")]
        public int EmployeeId { get; set; }
        [Column("client")]
        public int ClientId { get; set; }
        private bool isClosed;
        [Column("is_closed")]
        public bool IsClosed
        {
            get => isClosed;
            set
            {
                isClosed = value;
                RaisePropertyChanged(nameof(IsClosed));
            }
        }
        private string comment;
        [Column("comment")]
        public string Comment
        {
            get => comment;
            set
            {
                comment = value;
                RaisePropertyChanged(nameof(Comment));
            }
        }
        private double nostroRisk;
        [Column("nostro_risk_level")]
        public double NostroRiskLevel
        {
            get => nostroRisk;
            set
            {
                nostroRisk = value;
                RaisePropertyChanged(nameof(TotalRiskLevel));
                RaisePropertyChanged(nameof(TotalNostroLoroRiskLevel));
            }
        }
        private double _loroRisk;
        [Column("loro_risk_level")]
        public double LoroRiskLevel
        {
            get
            {
                return _loroRisk;
            }
            set
            {
                _loroRisk = value;
                RaisePropertyChanged(nameof(TotalRiskLevel));
                RaisePropertyChanged(nameof(TotalNostroLoroRiskLevel));
            }
        }
        [NotMapped]
        public string TotalRiskLevel
        {
            get
            {
                double res = Criterias.Select(i=>i.Criteria.Weight).Sum() + NostroRiskLevel + LoroRiskLevel;
                return res switch
                {
                    double n when n > 13.1 => $"{res:N1} - Критичный",
                    double n when n >= 5.5 && n <= 13.1 => $"{res:N1} - Высокий",
                    double n when n >= 3.4 && n < 5.5 => $"{res:N1} - Средний",
                    double n when n < 3.4 => $"{res:N1} - Низкий",
                    _ => "Не определено",
                };
            }
        }
        [NotMapped]
        public string TotalNostroLoroRiskLevel
        {
            get
            {
                double res = NostroRiskLevel + LoroRiskLevel;
                return res switch
                {
                    double n when n > 13.1 => $"{res:N1} - Критичный",
                    double n when n >= 5.5 && n <= 13.1 => $"{res:N1} - Высокий",
                    double n when n >= 3.4 && n < 5.5 => $"{res:N1} - Средний",
                    double n when n < 3.4 => $"{res:N1} - Низкий",
                    _ => "Не определено",
                };
            }
        }

        [ForeignKey(nameof(ClientId))]
        public virtual Client Client { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; }
        public ObservableCollection<ScoringToCriteria> Criterias { get; set; } = new ObservableCollection<ScoringToCriteria>();
    }
}
