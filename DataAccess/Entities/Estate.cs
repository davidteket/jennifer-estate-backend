using System.ComponentModel.DataAnnotations;

using System;

namespace DunakanyarHouseIngatlan.DataAccess.Entities
{
    public class Estate
    {
        [Key]
        public int Id { get; set; }
        public int Price { get; set; }
        public int TotalSquareFeet { get; set; }
        public int? LandSquareFeet { get; set; }
        public int RoomCount { get; set; }
        public int KitchenCount { get; set; }
        public int BathroomCount { get; set; }
        public int FloorCount { get; set; }
        public int GarageSquareFeet { get; set; }
        public int GardenSquareFeet { get; set; }
        public int TerraceSquareFeet { get; set; }
        
        public DateTime BuiltAt { get; set; }
        public DateTime? RefurbishedAt { get; set; }

        public bool HasElevator { get; set; }
        public bool HasGarden { get; set; }
        public bool HasGarage { get; set; }
        public bool HasDisabledFriendly { get; set; }
        public bool HasInnerHeightGreatherThan3Meters { get; set; }
        public bool HasSeparateWcAndBathroom { get; set; }
        public bool HasParticipatedInThePanelProgram { get; set; }
        public bool HasHeatIsolated { get; set; }
        public bool HasTerace { get; set; }       
        public bool HasBasement { get; set; }

        public string Roof { get; set; }
        public string Comfort { get; set; }
        public string Outlook { get; set; }
        public string Area { get; set; }
        public string Quality { get; set; }
        public string Category { get; set; }
        public string AdvertiserId { get; set; }
    }
}