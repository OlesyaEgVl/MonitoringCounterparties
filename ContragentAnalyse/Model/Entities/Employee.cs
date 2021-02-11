using ContragentAnalyse.Model.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContragentAnalyse.Model.Entities
{
    public class Employee : NamedEntity
    {
        public int? Position_Id { get; set; }
        public string CodeName { get; set; }

        [ForeignKey(nameof(Position_Id))]
        public virtual Positions Position { get; set; }

    }
}
