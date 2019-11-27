using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using backend.DataAccess.Entities;
using backend.DataAccess.Entities.Identity;

namespace backend.DataAccess
{
    public class JenniferEstateContext : IdentityDbContext<User, UserRole, string>
    {
        public DbSet<Address> Address { get; set; }
        public DbSet<Advertisement> Advertisement { get; set; }
        public DbSet<Client> Client { get; set; }
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

        // Identity
        //
        public DbSet<Role> Role { get; set; }
        public DbSet<UserClaim> Claim { get; set; }
        public DbSet<UserLogin> Login { get; set; }
        public DbSet<User> User { get; set; }

        public JenniferEstateContext(DbContextOptions options) : base(options)
        {
            if (this.Database.CanConnect())
                System.Console.WriteLine("Adatbázis kontextus létrehozva, a csatlakozás lehetséges.");
        }
    }
}