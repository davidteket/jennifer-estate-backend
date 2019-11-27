using System.ComponentModel.DataAnnotations;

namespace backend.DataAccess.Entities
{
    public class Electricity
    {
        [Key]
        public int Id { get; set; }
        public bool SunCollector { get; set; }
        public bool Thermal { get; set; }
        public bool Networked { get; set; }
        public int EstateId { get; set; }
    }
}