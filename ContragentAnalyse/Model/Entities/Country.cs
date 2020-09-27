using ContragentAnalyse.Model.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ContragentAnalyse.Model.Entities
{
    public class Country : NamedEntity
    {
        public int CountryCode { get; set; } 
        public string Code { get; set; }    //БУКВЕННЫЙ КОД
        public string Currency { get; set; }
        public List<Client> Client { get; set; }
    }
}
