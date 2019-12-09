using System.Collections.Generic;
using backend.DataAccess.Entities;

namespace backend.Models
{
    public class EstateDetailsViewModel
    {
            public Estate Estate { get; set; }
            public Electricity Electricity { get; set; }
            public HeatingSystem Heating { get; set; }
            public Address Address { get; set; }
            public WaterSystem Water { get; set; }
            public PublicService Services { get; set; }
            public Advertisement Advertisement { get; set; }
            public List<GenericImage> Images { get; set; }
    }
}