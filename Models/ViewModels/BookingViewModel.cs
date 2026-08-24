// Models/ViewModels/BookingViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Models.ViewModels
{
    public class BookingViewModel
    {
        public int VehicleId { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [DataType(DataType.Currency)]
        public decimal DailyPrice { get; set; }

        [DataType(DataType.Currency)]
        public decimal TotalPrice => DailyPrice * (decimal)(EndDate - StartDate).TotalDays;
    }
}