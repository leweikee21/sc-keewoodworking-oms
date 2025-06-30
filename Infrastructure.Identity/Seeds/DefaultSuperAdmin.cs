using Application.Enums;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto.Prng.Drbg;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity.Seeds
{
    public static class DefaultSuperAdmin
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Define the default super admin user
            var superAdminEmail = "superadmin@gmail.com";
            var existingUser = await userManager.FindByEmailAsync(superAdminEmail);

            if (existingUser == null)
            {
                var defaultUser = new ApplicationUser
                {
                    UserName = "superadmin",
                    FirstName = "Super",
                    LastName = "Admin",
                    Email = superAdminEmail,
                    PhoneNumber = "0108888888",
                    EmpNumber = "000001",
                    IsFirstLogin = false,
                    IsDeleted = false,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                var createResult = await userManager.CreateAsync(defaultUser, "Admin@123");

                if (createResult.Succeeded)
                {
                    // Retrieve the newly created user to ensure it has an ID
                    var newUser = await userManager.FindByEmailAsync(superAdminEmail);

                    if (newUser != null)
                    {
                        var roleExists = await roleManager.RoleExistsAsync(Roles.Admin.ToString());
                        if (!roleExists)
                        {
                            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
                        }

                        await userManager.AddToRoleAsync(newUser, Roles.Admin.ToString());
                    }
                }
                else
                {
                    // Log or handle user creation failure
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new System.Exception($"Failed to create super admin: {errors}");
                }
            }
        }
    }
}
