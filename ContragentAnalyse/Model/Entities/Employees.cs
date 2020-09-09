using System;
using System.Collections.Generic;
using System.Text;
using ContragentAnalyse.Model.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContragentAnalyse.Model.Entities
{
    public class Employees : NamedEntity
    {
        public int Position_Id { get; set; }

        [ForeignKey(nameof(Position_Id))]
        public virtual Positions Position { get; set; }

    }
}
