using System.Collections.Generic;

namespace DunakanyarHouseIngatlan.Models
{
    public class EmployeeDetailsViewModel
    {
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; } 
        public string ApproachType { get; set; } 
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public int AdvertisementCount { get; set; }
        public string ProfilePictureId { get; set; }
        public List<string> EmployeeRoles { get; set; }
    }
}