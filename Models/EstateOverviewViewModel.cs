namespace backend.Models
{
    public class EstateOverviewViewModel
    {
        public int EstateId { get; set; }
        public int Image { get; set; }
        public string Title { get; set; }
        public int Price { get; set; }
        public string OfferType { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Street { get; set; }
        public string Country { get; set; }
    }
}