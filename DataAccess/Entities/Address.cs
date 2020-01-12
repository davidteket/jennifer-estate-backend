using System.ComponentModel.DataAnnotations;

namespace DunakanyarHouseIngatlan.DataAccess.Entities
{ 
    public class Address
    {
        [Key]
        public int Id { get; set; }        
        public int? FloorNumber { get; set; }
        public int EstateId { get; set; }
        
        public string Country { get; set; }
        public string County { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string Door { get; set; }
    }
}