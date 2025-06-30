using Application.Enums;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity.Seeds
{
    public static class DefaultBasicUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await CreateUserAsync(userManager, roleManager,
                userName: "orderuser",
                firstName: "Order",
                lastName: "User",
                email: "orderuser@gmail.com",
                phone: "0128080456",
                empNumber: "123456",
                role: Roles.Order.ToString(),
                password: "Order@123");


            await CreateUserAsync(userManager, roleManager,
                userName: "inventoryuser",
                firstName: "Inventory",
                lastName: "User",
                email: "inventoryuser@gmail.com",
                phone: "0112233445",
                empNumber: "789012",
                role: Roles.Inventory.ToString(),
                password: "Inventory@123");
        }

        private static async Task CreateUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
                                                 string userName, string firstName, string lastName, string email, string phone, string empNumber, string role, string password)
        {
            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser == null) 
            {
                var user = new ApplicationUser
                {
                    UserName = userName,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PhoneNumber = phone,
                    EmpNumber = empNumber,
                    IsFirstLogin = false,
                    IsDeleted = false,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, password);

                if (createResult.Succeeded) 
                {
                    // Fetch the newly created user again to ensure the Id is populated
                    var newUser = await userManager.FindByEmailAsync(user.Email);

                    if (newUser != null)
                    {
                        var roleExists = await roleManager.RoleExistsAsync(role);
                        if (!roleExists)
                        {
                            await roleManager.CreateAsync(new IdentityRole(role));
                        }

                        await userManager.AddToRoleAsync(newUser, role);
                    }
                }
                else
                {
                    // Log errors if user creation fails
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create user: {errors}");
                }
            }
        }
    }
}
