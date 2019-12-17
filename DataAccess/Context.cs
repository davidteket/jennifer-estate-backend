using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using backend.DataAccess.Entities;
using backend.DataAccess.Entities.Identity;
using backend.Services;

namespace backend.DataAccess
{
    public class JenniferEstateContext : IdentityDbContext<User, UserRole, string>
    {
        private Serializer _serializer;
        
        // Entitások
        //
        public DbSet<Address> Address { get; set; }
        public DbSet<Advertisement> Advertisement { get; set; }
        public DbSet<ClientRequest> ClientRequest { get; set; }
        public DbSet<Electricity> Electricity { get; set; }
        public DbSet<Estate> Estate { get; set; }
        public DbSet<EstateClient> EstateClient { get; set; }
        public DbSet<GenericImage> GenericImage { get; set; }
        public DbSet<HeatingSystem> HeatingSystem { get; set; }
        public DbSet<Popularity> Popularity { get; set; }
        public DbSet<PublicService> PublicService { get; set; }
        public DbSet<WaterSystem> WaterSystem { get; set; }
        public DbSet<CompanyDetails> CompanyDetails { get; set; }
        public DbSet<Invitation> Invitation { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<UserClaim> Claim { get; set; }
        public DbSet<UserLogin> Login { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserRole> UserRole { get; set; }

        public JenniferEstateContext(DbContextOptions options) : base(options)
        {
            _serializer = new Serializer();
            if (this.Database.CanConnect())
                System.Console.WriteLine(_serializer.GetServerLogMessage("CanConnectToDatabase"));
            else
                System.Console.WriteLine(_serializer.GetServerLogMessage("CannotConnectToDatabase"));
        }
    }
}