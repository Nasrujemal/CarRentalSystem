using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalSystem.Infrastructure;
using CarRentalSystem.Models.Entities;

public class BookingsController : Controller
{
    private readonly ApplicationDbContext _context;

    public BookingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var bookings = _context.Bookings
            .Include(b => b.User)       // Include related user
            .Include(b => b.Vehicle)    // Include related vehicle
            .ToList();

        return View(bookings);
    }
}