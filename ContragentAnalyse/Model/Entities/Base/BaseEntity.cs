using System.ComponentModel.DataAnnotations;

namespace ContragentAnalyse.Model.Entities.Base
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
