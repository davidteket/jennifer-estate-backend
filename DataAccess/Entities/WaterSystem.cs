using System.ComponentModel.DataAnnotations;

namespace backend.DataAccess.Entities
{
    public class WaterSystem
    {
        [Key]
        public int Id { get; set; }
        public string AvailabilityType { get; set; }
        public int EstateId { get;  set; }
    }
}