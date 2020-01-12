using DunakanyarHouseIngatlan.DataAccess.Entities;

namespace DunakanyarHouseIngatlan.Models
{
    public class UploadWrapperModel
    {
        public Estate Estate { get; set; }
        public Address Address { get; set; }
        public Electricity Electricity { get; set; }
        public HeatingSystem Heating { get; set; }
        public PublicService PublicService { get; set; }
        public Advertisement Advertisement { get; set; }
    }
}