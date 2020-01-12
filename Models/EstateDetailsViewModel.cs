using System.Collections.Generic;
using DunakanyarHouseIngatlan.DataAccess.Entities;

namespace DunakanyarHouseIngatlan.Models
{
    public class EstateDetailsViewModel
    {
            public Estate Estate { get; set; }
            public Electricity Electricity { get; set; }
            public HeatingSystem Heating { get; set; }
            public Address Address { get; set; }
            public PublicService Services { get; set; }
            public Advertisement Advertisement { get; set; }
            public List<GenericImage> Images { get; set; }
    }
}