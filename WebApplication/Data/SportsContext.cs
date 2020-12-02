using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;

namespace WebApplication.Data
{
    public class SportsContext : IdentityDbContext<Person, Role, int>
    {
        public SportsContext(DbContextOptions<SportsContext> options) : base(options)
        {
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Boat> Boats { get; set; }
        public DbSet<BoatOwner> BoatOwners { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingLine> BookingLines { get; set; }
        public DbSet<Marina> Marinas { get; set; }
        public DbSet<MarinaOwner> MarinaOwners { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Spot> Spots { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Person>().HasOne<BoatOwner>(p => p.BoatOwner).WithOne(bO => bO.Person).HasForeignKey<BoatOwner>(bO => bO.PersonId);
            modelBuilder.Entity<Person>().HasOne<MarinaOwner>(p => p.MarinaOwner).WithOne(bO => bO.Person).HasForeignKey<MarinaOwner>(bO => bO.PersonId);

            modelBuilder.Entity<BoatOwner>().HasOne(bO => bO.Person).WithOne().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<MarinaOwner>().HasOne(mo => mo.Person).WithOne().OnDelete(DeleteBehavior.NoAction);

            // Singular Table Names
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Use the entity name instead of the Context.DbSet<T> name refs https://docs.microsoft.com/en-us/ef/core/modeling/entity-types?tabs=fluent-api#table-name
                modelBuilder.Entity(entityType.ClrType).ToTable(entityType.ClrType.Name);
            }
        }
    }
}
