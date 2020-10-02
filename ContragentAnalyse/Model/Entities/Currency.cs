using System;
using System.Collections.Generic;
using System.Text;
using ContragentAnalyse.Model.Entities.Base;

namespace ContragentAnalyse.Model.Entities
{
    public class Currency: NamedEntity
    {
        public int CodeCurrency { get; set; }    //числовой КОД

        public List<Client> Client { get; set; }

    }
}
