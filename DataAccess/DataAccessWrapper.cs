using backend.DataAccess.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace backend.DataAccess
{
    public class DataAccessWrapper
    {
        public UserManager<User> UserManager { get; set; }
    }
}