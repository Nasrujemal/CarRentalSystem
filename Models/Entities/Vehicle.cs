using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Models.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }
        public required string Make { get; set; }      // e.g., Toyota, Ford, etc.
        public required string Model { get; set; }     // e.g., Camry, Fiesta, etc.
   
        [Range(1900, 2024)]
        public int Year { get; set; } = DateTime.Now.Year;

        [DataType(DataType.Currency)]
        public decimal PricePerDay { get; set; }
        public required string ImagePath { get; set; } // For vehicle image uploads
        public bool IsAvailable { get; set; } = true;
    }
}
