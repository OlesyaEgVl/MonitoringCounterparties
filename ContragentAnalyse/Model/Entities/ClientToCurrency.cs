using ContragentAnalyse.Model.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
namespace ContragentAnalyse.Model.Entities
{
    public class ClientToCurrency : BaseEntity
    {
        public int Currency_Id { get; set; } //почему они - Nullable типы? Если нет валюты или клиента запись не имеет значения
        public int Client_Id { get; set; } //WAT? :)
        [ForeignKey(nameof(Client_Id))]
        public virtual Client Client { get; set; }
        [ForeignKey(nameof(Currency_Id))]
        public Currency Currency { get; set; }

    }
}
