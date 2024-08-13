using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.Models
{
    public class AppUser:IdentityUser
    {
        public string? City { get; set; }
        public string? Picture { get; set; }
        public DateTime? Birthday { get; set; }
        public byte? Gender { get; set; }
    }
}
