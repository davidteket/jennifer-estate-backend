using System.ComponentModel.DataAnnotations;

namespace backend.DataAccess.Entities
{
    public class HeatingSystem
    {
        [Key]
        public int Id { get; set; }
        public bool ByWood { get; set; }
        public bool ByRemote { get; set; }
        public bool ByGas { get; set; }
        public bool ByElectricity { get; set; }
        public bool FloorHeating { get; set; }
        public int EstateId { get; set; }
    }
}