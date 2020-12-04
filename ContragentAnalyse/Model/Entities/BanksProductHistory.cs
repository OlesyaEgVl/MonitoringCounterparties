using ContragentAnalyse.Model.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace ContragentAnalyse.Model.Entities
{
    public class BanksProductHistory : NamedEntity
    {
        public int Client_Id { get; set; }
        public string NOSTRO { get; set; }
        public string LORO { get; set; }
        public string Level_NostroLoro { get; set; }
        [ForeignKey(nameof(Client_Id))]
        public virtual Client Client { get; set; }
    }
}
