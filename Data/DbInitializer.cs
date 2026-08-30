using InventoryManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Manager", "Staff" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        /// <summary>
        /// Seeds a default Admin account if no Admin user exists.
        /// Credentials: admin@inventory.com / Admin1234
        /// Change the password immediately after first login.
        /// </summary>
        public static async Task SeedAdminUser(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(roleManager);

            const string adminEmail    = "admin@inventory.com";
            const string adminPassword = "Admin1234";

            var existing = await userManager.FindByEmailAsync(adminEmail);
            if (existing != null) return; // already seeded

            var admin = new ApplicationUser
            {
                UserName       = adminEmail,
                Email          = adminEmail,
                FullName       = "System Administrator",
                Address        = "",
                Country        = "",
                PhoneNumber    = "",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
