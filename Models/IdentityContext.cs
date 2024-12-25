using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Models
{
    public class IdentityContext: IdentityDbContext<AppUser, AppRole, string>
    {
        public required DbSet<ServiceRecord> ServiceRecords { get; set; }
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
            
        }

        
    }

}