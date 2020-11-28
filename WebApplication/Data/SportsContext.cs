using Microsoft.EntityFrameworkCore;
using WebApplication.Models;

namespace WebApplication.Data
{
    public class SportsContext : DbContext
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
            modelBuilder.Entity<Address>().ToTable("Address");
            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<BoatOwner>().ToTable("BoatOwner");
            modelBuilder.Entity<BoatOwner>().HasOne(bO => bO.Person).WithOne().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Boat>().ToTable("Boat");
            modelBuilder.Entity<MarinaOwner>().ToTable("MarinaOwner");
            modelBuilder.Entity<MarinaOwner>().HasOne(mo => mo.Person).WithOne().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Marina>().ToTable("Marina");
            modelBuilder.Entity<Review>().ToTable("Review");
            modelBuilder.Entity<Spot>().ToTable("Spot");
            modelBuilder.Entity<Booking>().ToTable("Booking");
            modelBuilder.Entity<BookingLine>().ToTable("BookingLine");
            modelBuilder.Entity<Location>().ToTable("Location");
            modelBuilder.Entity<Payment>().ToTable("Payment");
        }
    }
}
