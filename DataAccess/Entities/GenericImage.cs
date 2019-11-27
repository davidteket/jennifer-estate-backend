using System.ComponentModel.DataAnnotations;

namespace backend.DataAccess.Entities
{
    public class GenericImage 
    {
        [Key]
        public int Id { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string DescriptionDetail { get; set; }
        public int StorageSize { get; set; }
        public string Resolution { get; set; }
        public int? EstateId { get; set; }
        public string UserId { get;  set; }
    }
}