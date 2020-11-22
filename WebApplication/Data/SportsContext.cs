﻿using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
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
        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await base.SaveChangesAsync(cancellationToken);
                if (result < 1)
                    throw new BusinessException("Error", "Nothing was modified.");

                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new BusinessException("Error", "Database problems, couldn't save changes.\n" + ex.ToString());
            }
            catch (DbUpdateException ex)
            {
                throw new BusinessException("Error", "Concurrency problems, couldn't save change.\n" + ex.ToString());
            }
        }
    }
}
