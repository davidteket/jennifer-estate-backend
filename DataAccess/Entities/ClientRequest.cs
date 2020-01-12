using System.ComponentModel.DataAnnotations;

namespace DunakanyarHouseIngatlan.DataAccess.Entities
{
    public class ClientRequest
    {
        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }

        public bool IsReasonSellRequest { get; set; }
        public bool IsReasonPhotoRequest { get; set; }
        public bool IsReasonPriceCheckRequest { get; set; }
        public bool IsReasonAdviceRequest { get; set; }
    }
}