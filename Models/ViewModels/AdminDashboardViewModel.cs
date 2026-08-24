// Models/ViewModels/AdminDashboardViewModel.cs
using CarRentalSystem.Models.Entities;

namespace CarRentalSystem.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalBookings { get; set; }
        public int ActiveVehicles { get; set; }
        public IList<Booking> RecentBookings { get; set; } = new List<Booking>();

    }
}