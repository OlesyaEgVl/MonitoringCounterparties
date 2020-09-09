using ContragentAnalyse.Model.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContragentAnalyse.Model.Entities
{
    public class Contracts : NamedEntity
    {
        public int Bank_Id { get; set; }
        [ForeignKey(nameof(Bank_Id))]
        public virtual Bank Bank { get; set; }

    }
}
