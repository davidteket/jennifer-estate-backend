namespace backend.DataAccess.Entities
{
    public class CompanyDetails
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public string CompanyEmailAddress { get; set; }
        public string AboutUs { get; set; }
        
        public string InvitationSubject { get; set; }
        public string InvitationMessage { get; set; }
    }
}