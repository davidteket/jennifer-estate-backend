using DunakanyarHouseIngatlan.DataAccess.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace DunakanyarHouseIngatlan.DataAccess
{
    public class DataAccessWrapper
    {
        public UserManager<User> UserManager { get; set; }
    }
}