using ContragentAnalyse.Model.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;
namespace ContragentAnalyse.Model.Entities
{
    public class ClientToCurrency : BaseEntity
    {
        public int Currency_Id { get; set; }
        public int Client_Id { get; set; }
        [ForeignKey(nameof(Client_Id))]
        public virtual Client Client { get; set; }
        [ForeignKey(nameof(Currency_Id))]
        public Currency Currency { get; set; }

    }
}
