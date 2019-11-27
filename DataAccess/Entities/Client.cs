using System.ComponentModel.DataAnnotations;

namespace backend.DataAccess.Entities
{
    public class Client
    {
        [Key]
        public int Id { get; set; }
        public bool CallbackRequested { get; set; }
    }
}