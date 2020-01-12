using System;
using Microsoft.AspNetCore.Identity;

namespace DunakanyarHouseIngatlan.DataAccess.Entities.Identity
{
    public class User : IdentityUser 
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }

        public User()
        {
            base.Id = Guid.NewGuid().ToString();
        }
    }
}