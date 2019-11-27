using System.ComponentModel.DataAnnotations;

using System;

namespace backend.DataAccess.Entities
{
    public class Advertisement
    {
        [Key]
        public int Id { get; set; }
        public DateTime TimePosted { get; set; }
        public string Title { get; set; }
        public string DescriptionDetail { get; set; }
        public DateTime? LastModification { get; set; }
        public int? OrderOfAppearance { get; set; }
        public string OfferType { get; set; }
        public int EstateId { get; set; }
        public string AdvertiserId { get; set; }
    }
}