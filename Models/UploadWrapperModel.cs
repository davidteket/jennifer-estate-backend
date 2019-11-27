using backend.DataAccess.Entities;

namespace backend.Models
{
    public class UploadWrapperModel
    {
        public Estate Estate { get; set; }
        public Address Address { get; set; }
        public Electricity Electricity { get; set; }
        public HeatingSystem Heating { get; set; }
        public PublicService PublicService { get; set; }
        public WaterSystem Water { get; set; }
        public Advertisement Advertisement { get; set; }
    }
}