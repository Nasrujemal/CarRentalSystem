using CarRentalSystem.Models.Entities;  // For Booking
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace CarRentalSystem.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public required string FirstName { get; set; } = string.Empty;
        public required string LastName { get; set; } = string.Empty;

        // Initialize Bookings to prevent null reference errors
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
