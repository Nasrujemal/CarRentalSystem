using System;
using CarRentalSystem.Models.Enums;
using CarRentalSystem.Models.Identity; // Ensure this using is present

namespace CarRentalSystem.Models.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        // Foreign key for the vehicle
        public int VehicleId { get; set; }
        public required Vehicle Vehicle { get; set; }

        // Foreign key for the user (customer)
        public required string UserId { get; set; }
        public required ApplicationUser User { get; set; }

        // Booking period
        public DateTime StartDate { get; set; } // When the booking starts
        public DateTime EndDate { get; set; }   // When the booking ends

        // Calculated total price for the booking
        public decimal TotalPrice { get; set; }

        // Booking status using the BookingStatus enum
        public BookingStatus Status { get; set; }
    }
}
