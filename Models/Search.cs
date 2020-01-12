namespace DunakanyarHouseIngatlan.Models
{
    public class Search
    {
        public int Price { get; set; }
        public int SquareFeet { get; set; }
        public int BuiltAt { get; set; }
        public int RefurbishedAt { get; set; }
        public int RoomCount { get; set; }
        public int KitchenCount { get; set; }
        public int BathroomCount { get; set; }
        public int FloorCount { get; set; }
        public int LandSquareFeet { get; set; }
        // Cont. from here 

        public string Category { get; set; }
        public string Quality { get; set; }

    }
}