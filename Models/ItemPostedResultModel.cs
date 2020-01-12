namespace DunakanyarHouseIngatlan.Models
{
    public class ItemPostedResultModel
    {
        public string ItemId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public ItemPostedResultModel()
        {
            ItemId = null;
            Success = false;
            Message = null;
        }
    }
}