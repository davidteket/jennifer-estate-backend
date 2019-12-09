using System.ComponentModel.DataAnnotations;

namespace backend.DataAccess.Entities
{
    public class ClientRequest
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public int ReasonSellRequest { get; set; }
        public int ReasonPhotoRequest { get; set; }
        public int ReasonPriceCheckRequest { get; set; }
        public int ReasonAdviceRequest { get; set; }
    }
}