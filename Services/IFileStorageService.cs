using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CarRentalSystem.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveVehicleImage(IFormFile file);
    }
}
