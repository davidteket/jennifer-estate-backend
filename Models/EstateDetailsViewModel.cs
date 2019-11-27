using System;
using System.Collections.Generic;

namespace backend.Models
{
    public class EstateDetailsViewModel
    {
        public int SquareFeet { get; set; }
        public string Category { get; set; }
        public DateTime BuiltAt { get; set; }
        public string RefurbishedAt { get; set; }
        public string Grade { get; set; }
        public int Room { get; set; }
        public int Kitchen { get; set; }
        public int Bathroom { get; set; }
        public int FloorCount { get; set; }
        public int Garage { get; set; }
        public int Elevator { get; set; }
        public int Terrace { get; set; }
        public int PropertySquareFeet { get; set; }
        public int GarageSquareFeet { get; set; }
        public int GardenSquareFeet { get; set; }
        public int TerraceSquareFeet { get; set; }
        public int Basement { get; set; }
        public string Comfort { get; set; }
        public string Advertiser { get; set; }
        public int AdvertiserId { get; set; }
        public List<int> Images { get; set; }
    }
}