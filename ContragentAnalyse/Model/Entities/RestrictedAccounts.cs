using System;
using System.Collections.Generic;
using System.Text;
using ContragentAnalyse.Model.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;


namespace ContragentAnalyse.Model.Entities
{
    public class RestrictedAccounts : BaseEntity
    {
        public int Bank_Id { get; set; }
        public int Currency_Id { get; set; }
        public string AccountNumber { get; set; }
        public int AccountState_Id { get; set; }

        [ForeignKey(nameof(Currency_Id))]
        public virtual Currency Currency { get; set; }

        [ForeignKey(nameof(Bank_Id))]
        public virtual Bank Bank { get; set; }

        [ForeignKey(nameof(AccountState_Id))]
        public virtual AccountStates AccountState { get; set; }
    }
}
