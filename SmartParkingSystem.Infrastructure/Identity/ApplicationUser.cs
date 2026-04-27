using Microsoft.AspNetCore.Identity;

namespace SmartParkingSystem.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        
        // Example of a custom field:
        public string? DefaultVehicleNumber { get; set; }
    }
}
