using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalSystem.Infrastructure;
using CarRentalSystem.Models.Entities;
using CarRentalSystem.Models.ViewModels;
using CarRentalSystem.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CarRentalSystem.Models.Enums;
using System;
using System.Threading.Tasks;

namespace CarRentalSystem.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public BookingsController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await _context.Bookings
                .Include(b => b.Vehicle)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Bookings/Create?vehicleId=5
        public IActionResult Create(int vehicleId)
        {
            var vehicle = _context.Vehicles.FirstOrDefault(v => v.Id == vehicleId && v.IsAvailable);
            if (vehicle == null)
            {
                TempData["Error"] = "Vehicle not available for booking";
                return RedirectToAction("Index", "Vehicles");
            }

            var model = new BookingViewModel
            {
                VehicleId = vehicleId,
                DailyPrice = vehicle.PricePerDay,
                //MinDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd")
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await _context.Users.FindAsync(userId);
                    var vehicle = await _context.Vehicles.FindAsync(model.VehicleId);

                    // Validate required relationships
                    if (user == null || vehicle == null)
                    {
                        TempData["Error"] = "Invalid user or vehicle";
                        return RedirectToAction("Index", "Vehicles");
                    }

                    // Initialize required members
                    var booking = new Booking
                    {
                        UserId = userId,
                        User = user,            // Required navigation property
                        VehicleId = model.VehicleId,
                        Vehicle = vehicle,      // Required navigation property
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        TotalPrice = CalculateTotalPrice(model.StartDate, model.EndDate, vehicle.PricePerDay),
                        Status = BookingStatus.Confirmed
                    };

                    _context.Bookings.Add(booking);
                    await _context.SaveChangesAsync();

                    await _emailService.SendBookingConfirmation(booking);
                    return RedirectToAction("Details", new { id = booking.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while processing your request.");
                    return View(model);
                }
            }
            return View(model);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Vehicle)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            // Authorization check
            if (booking.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            return View(booking);
        }

        private decimal CalculateTotalPrice(DateTime start, DateTime end, decimal dailyPrice)
        {
            var days = (end - start).Days;
            return days * dailyPrice;
        }
    }
}