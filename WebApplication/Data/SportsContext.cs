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

        //TODO: Add DbSets once Model is done

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>().ToTable("Address");
            modelBuilder.Entity<Boat>().ToTable("Boat");
            modelBuilder.Entity<BoatOwner>().ToTable("BoatOwner");
            modelBuilder.Entity<Booking>().ToTable("Booking");
            modelBuilder.Entity<BookingLine>().ToTable("BookingLine");
            modelBuilder.Entity<Marina>().ToTable("MarinaOwners");
            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<Review>().ToTable("Review");
            modelBuilder.Entity<Spot>().ToTable("Spot");
        }
    }
}
