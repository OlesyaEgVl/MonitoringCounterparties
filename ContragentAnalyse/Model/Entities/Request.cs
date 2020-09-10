using ContragentAnalyse.Model.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ContragentAnalyse.Model.Entities
{
    [Table("Requests")]
    public class Request :BaseEntity
    {
        public int Client_Id { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime? RecieveDate { get; set; }
        public string Comment { get; set; }
        [ForeignKey(nameof(Client_Id))]
        public virtual Client Client { get; set; }
    }
}
