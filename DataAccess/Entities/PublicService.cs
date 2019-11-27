using System.ComponentModel.DataAnnotations;

namespace backend.DataAccess.Entities
{
    public class PublicService
    {
        [Key]
        public int Id { get; set; }
        public int Grocery { get; set; }
        public int GasStation { get; set; }
        public string Transport { get; set; }
        public int DrugStore { get; set; }
        public int School { get; set; }
        public int MailDepot { get; set; }
        public int Bank { get; set; }
        public int EstateId { get; set; }
    }
}