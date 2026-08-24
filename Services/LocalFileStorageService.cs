using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace CarRentalSystem.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;

        public LocalFileStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveVehicleImage(IFormFile file)
        {
            var uploadsPath = Path.Combine(_env.WebRootPath, "images", "vehicles");
            Directory.CreateDirectory(uploadsPath);

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/vehicles/{uniqueFileName}";
        }
    }
}
