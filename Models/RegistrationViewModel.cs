using backend.DataAccess.Entities.Identity;

namespace backend.Models
{
    public class RegistrationViewModel
    {
        public User User { get; set; }
        public backend.DataAccess.Entities.Invitation Invitation { get; set; }
        public string Password { get; set; }
    }
}