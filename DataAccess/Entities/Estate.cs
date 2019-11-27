using System.ComponentModel.DataAnnotations;

using System;

namespace backend.DataAccess.Entities
{
    public class Estate
    {
        [Key]
        public int Id { get; set; }
        public int Price { get; set; }
        public int SquareFeet { get; set; }
        public string Category { get; set; }
        public DateTime BuiltAt { get; set; }
        public DateTime? RefurbishedAt { get; set; }
        public string Grade { get; set; }
        public int Room { get; set; }
        public int Kitchen { get; set; }
        public int Bathroom { get; set; }
        public int FloorCount { get; set; }
        public int Garage { get; set; }
        public int Elevator { get; set; }
        public int Garden { get; set; }
        public int Terace { get; set; }
        public int PropertySquareFeet { get; set; }
        public int? GarageSquareFeet { get; set; }
        public int? GardenSquareFeet { get; set; }
        public int? TerraceSquareFeet { get; set; }
        public int Basement { get; set; }
        public string Comfort { get; set; }
        public string AdvertiserId { get; set; }
    }
}