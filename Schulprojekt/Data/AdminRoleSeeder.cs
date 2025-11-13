using Microsoft.AspNetCore.Identity;

namespace Schulprojekt.Data
{
    public class AdminRoleSeeder
    {
        
        public static async Task AdminRoleSeederAsync(RoleManager<IdentityRole> _roleManager, UserManager<IdentityUser> _userManager){

            if (! await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
            }


            if (await _userManager.FindByEmailAsync("admin@admin.com") == null)
            {
                var user = new IdentityUser
                {
                    Email = "admin@admin.com",
                    UserName = "admin@admin.com",
                };
                var result = await _userManager.CreateAsync(user, "Admin!123");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
