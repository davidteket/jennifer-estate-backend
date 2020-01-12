using System.ComponentModel.DataAnnotations;

namespace DunakanyarHouseIngatlan.DataAccess.Entities
{
    public class PublicService
    {
        [Key]
        public int Id { get; set; }
        public int EstateId { get; set; }
        
        public bool HasGroceryNearby { get; set; }
        public bool HasGasStationNearby { get; set; }
        public bool HasTransportNearby { get; set; }
        public bool HasDrugStoreNearby { get; set; }
        public bool HasSchoolNearby { get; set; }
        public bool HasMailDepotNearby { get; set; }
        public bool HasBankNearby { get; set; }
        public bool HasEntertainmentServicesNearby { get; set; }
    }
}