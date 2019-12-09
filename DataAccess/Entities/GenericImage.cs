using System.ComponentModel.DataAnnotations;

namespace backend.DataAccess.Entities
{
    public class GenericImage 
    {
        [Key]
        public string Id { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string DescriptionDetail { get; set; }
        public int StorageSize { get; set; }
        public string Extension { get; set; }
        public int? EstateId { get; set; }
        public string UserId { get;  set; }
    }
}