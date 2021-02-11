using ContragentAnalyse.Model.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;


namespace ContragentAnalyse.Model.Entities
{
    public class RestrictedAccounts : BaseEntity
    {
        public int Client_Id { get; set; }
        public int Currency_Id { get; set; }
        public string AccountNumber { get; set; }
        public int AccountState_Id { get; set; }

        [ForeignKey(nameof(Currency_Id))]
        public virtual Currency Currency { get; set; }

        [ForeignKey(nameof(Client_Id))]
        public virtual Client Client { get; set; }

        [ForeignKey(nameof(AccountState_Id))]
        public virtual AccountStates AccountState { get; set; }
    }
}
