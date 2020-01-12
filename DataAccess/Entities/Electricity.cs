using System.ComponentModel.DataAnnotations;

namespace DunakanyarHouseIngatlan.DataAccess.Entities
{
    public class Electricity
    {
        [Key]
        public int Id { get; set; }
        public int EstateId { get; set; }
        
        public bool SunCollector { get; set; }
        public bool PowerWall { get; set; }
        public bool Networked { get; set; }
    }
}