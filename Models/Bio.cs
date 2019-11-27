namespace backend.Models
{
    public class Bio 
    {
        public string UserId;
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ApproachType { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
    }
}