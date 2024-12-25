using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }

        public string? Field { get; set;}

        public AppRole? Role {get; set;}

    }
}