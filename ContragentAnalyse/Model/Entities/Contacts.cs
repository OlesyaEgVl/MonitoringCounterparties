using ContragentAnalyse.Model.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContragentAnalyse.Model.Entities
{
    public class Contacts : BaseEntity
    {
        public int ContactType_Id { get; set; }
        public string Value { get; set; }
        public string ContactFIO { get; set; }
        public int Client_Id { get; set; }

        [ForeignKey(nameof(Client_Id))]
        public virtual Client Client { get; set; }

        [ForeignKey(nameof(ContactType_Id))]
        public virtual ContactType ContactType { get; set; }
    }
}
