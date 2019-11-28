namespace backend.DataAccess.Entities
{
    public class Invitation 
    {
        public string Id { get; set; }
        public int Expired { get; set; }
        public string InviteeId { get; set; }

        public Invitation()
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.Expired = 0;
        }
    }
}