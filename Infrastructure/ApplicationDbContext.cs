using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CarRentalSystem.Models.Entities;
using CarRentalSystem.Models.Identity;

namespace CarRentalSystem.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet for Vehicles
        public DbSet<Vehicle> Vehicles { get; set; }

        // DbSet for Bookings
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call base method to configure Identity-related mappings.
            base.OnModelCreating(modelBuilder);

            // Configure Booking-vehicle relationship (each booking is associated with one vehicle)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Vehicle)
                .WithMany() // You can also add a navigation property in Vehicle if needed.
                .HasForeignKey(b => b.VehicleId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent accidental deletion of vehicles

            // Configure Booking-ApplicationUser relationship (each booking is linked to one user)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent accidental deletion of users

            // Enforce required fields for Vehicle
            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Make)
                .IsRequired();

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Model)
                .IsRequired();

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.ImagePath)
                .IsRequired();

            // Configure precision for decimal properties to avoid truncation
            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasPrecision(18, 2); // 18 total digits, 2 after the decimal

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.PricePerDay)
                .HasPrecision(18, 2);
        }
    }
}
