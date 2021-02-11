using ContragentAnalyse.Model.Entities.Base;

namespace ContragentAnalyse.Model.Entities
{
    public class Country : NamedEntity
    {
        public int CountryCode { get; set; } 
        public string Code { get; set; }
    }
}
