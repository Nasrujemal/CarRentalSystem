using CarRentalSystem.Models.Entities;
using System.Threading.Tasks;

namespace CarRentalSystem.Services
{
    public interface IEmailService
    {
        Task SendBookingConfirmation(Booking booking);
    }
}
