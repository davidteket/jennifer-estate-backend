using System.ComponentModel.DataAnnotations;

namespace backend.DataAccess.Entities
{
    public class EstateClient
    {
        [Key]
        public int EstateId { get; set; }
        public int ClientId { get; set; }
    }
}