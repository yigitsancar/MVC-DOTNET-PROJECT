using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Models
{
    public static class IdentitySeedData
    {
        private const string adminUser = "admin";
        private const string adminPassword = "Admin_123";
        private const string adminRole = "Admin";  // Define Admin role name

        public static async void IdentityTestUser(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<IdentityContext>();

            if (context.Database.GetAppliedMigrations().Any())
            {
                context.Database.Migrate();
            }

            var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

            // Check if the Admin role exists, if not, create it
            var roleExist = await roleManager.RoleExistsAsync(adminRole);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new AppRole { Name = adminRole });
            }

            // Check if the admin user exists
            var user = await userManager.FindByNameAsync(adminUser);

            if (user == null)
            {
                // Create the admin user if it doesn't exist with the specific details
                user = new AppUser
                {
                    FullName = "Azra Yılmaz",            // Set Full Name             
                    Email = "admin@gmail.com",           // Set Email
                    PhoneNumber = "44444444"             // Set Phone Number
                };

                await userManager.CreateAsync(user, adminPassword);  // Create the user with the password

                // After the user is created, add them to the Admin role
                await userManager.AddToRoleAsync(user, adminRole);
            }
            else
            {
                // If the user exists but is not in the Admin role, add them to the Admin role
                if (!await userManager.IsInRoleAsync(user, adminRole))
                {
                    await userManager.AddToRoleAsync(user, adminRole);
                }
            }
        }
    }
}
