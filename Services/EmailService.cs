using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using CarRentalSystem.Models.Entities;

namespace CarRentalSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendBookingConfirmation(Booking booking)
        {
            var emailSettings = _config.GetSection("EmailSettings");
            var client = new SmtpClient(emailSettings["SmtpServer"], int.Parse(emailSettings["Port"]!))
            {
                Credentials = new NetworkCredential(
                    emailSettings["Username"],
                    emailSettings["Password"]),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(emailSettings["Username"]!),
                Subject = "Booking Confirmation",
                Body = $"Your booking for {booking.Vehicle.Make} {booking.Vehicle.Model} " +
                       $"from {booking.StartDate:d} to {booking.EndDate:d} has been confirmed."
            };

            mail.To.Add(booking.User.Email!);
            await client.SendMailAsync(mail);
        }
    }
}