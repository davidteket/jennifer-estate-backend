using System.ComponentModel.DataAnnotations;

namespace DunakanyarHouseIngatlan.DataAccess.Entities
{
    public class HeatingSystem
    {
        [Key]
        public int Id { get; set; }       
        public int EstateId { get; set; }

        public bool ByCirculation { get; set; }
        public bool ByGasConvector { get; set; }
        public bool ByGas { get; set; }
        public bool ByWood { get; set; }
        public bool ByCombined { get; set; }
        public bool ByChimney { get; set; }
        public bool ByCockle { get; set; }
        public bool ByRemote { get; set; }
        public bool ByNetworked { get; set; }
        public bool ByElectricity { get; set; }
        public bool ByFloorHeating { get; set; }
    }
}