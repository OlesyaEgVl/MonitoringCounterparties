using ContragentAnalyse.Model.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContragentAnalyse.Model.Entities
{
    public class Contracts : NamedEntity
    {
        public int Client_Id { get; set; }
        [ForeignKey(nameof(Client_Id))]
        public virtual Client Client { get; set; }

    }
}
