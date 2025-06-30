using Application.DTOs.Account;
using Application.DTOs.User;
using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<PagedResponse<List<UserDto>>> GetAllUsersAsync(int pageNumber, int pageSize, string? search = null, string? role = null)
        {
            var query = _userManager.Users
                            .AsQueryable()
                            .Where(i => !i.IsDeleted);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => EF.Functions.Like(u.UserName, $"%{search}%")
                          || EF.Functions.Like(u.Email, $"%{search}%"));
            }

            if (!string.IsNullOrEmpty(role))
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                var userIdsInRole = usersInRole.Select(u => u.Id).ToList();
                query = query.Where(u => userIdsInRole.Contains(u.Id));
            }

            var totalUsers = await query.CountAsync();
            var users = query
                .OrderBy(u => u.FirstName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    EmpNumber = user.EmpNumber,
                    Roles = roles.ToList()
                });
            }

            return new PagedResponse<List<UserDto>>(userDtos, pageNumber, pageSize, totalUsers);
        }

        public async Task<List<string>> GetAllUserNamesAsync()
        {
            return await _userManager.Users
                .Where(i => !i.IsDeleted)
                .Select(user => user.UserName)
                .ToListAsync();
        }

        public async Task<Response<UserDto>> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ApiException("User not found");

            var roles = await _userManager.GetRolesAsync(user);

            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmpNumber = user.EmpNumber,
                Roles = roles.ToList()
            };

            return new Response<UserDto>(userDto);
        }

        public async Task<List<Response<UserDto>>> GetUsersByRoleAsync(string roleName)
        {
            var usersList = await _userManager.GetUsersInRoleAsync(roleName);
            var users = usersList.Where(i => !i.IsDeleted);

            if (users == null) 
                throw new ApiException($"Users with role '{roleName}' not found.");

            var responseList = new List<Response<UserDto>>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userDto = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    EmpNumber = user.EmpNumber,
                    Roles = roles.ToList()
                };

                responseList.Add(new Response<UserDto>(userDto));
            }

            return responseList;
        }

        public async Task<Response<string>> CreateUserAsync(RegisterRequest request)
        {
            var validationErrors = new List<string>();

            //Check Email
            var emailUser = await _userManager.FindByEmailAsync(request.Email);
            if (emailUser != null)
                validationErrors.Add("This email address is already taken. Please try another.");
            
            if (emailUser!= null && emailUser.IsDeleted && emailUser.FirstName == request.FirstName && emailUser.LastName == request.LastName)
                return Response<string>.WithException(emailUser.Id, "This user was previously deleted. Reactivate?", "ReactivateException");

            // Check Username
            var usernameUser = await _userManager.FindByNameAsync(request.UserName);
            if (usernameUser != null)
                validationErrors.Add("This username is already taken. Please try another.");

            // Check EmpNumber
            var empUser = await _userManager.Users
                .Where(u => u.EmpNumber == request.EmpNumber)
                .FirstOrDefaultAsync();
            if (empUser != null)
                validationErrors.Add("This employee number is already taken. Please try another.");

            if (validationErrors.Any())
                throw new ValidationException(validationErrors.Select(e => new FluentValidation.Results.ValidationFailure("", e)));

            //Else: Create new user
            var user = new ApplicationUser
            {
                UserName = request.UserName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                EmpNumber = request.EmpNumber,
                IsFirstLogin = true,
                IsDeleted = false,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new ApiException("Failed to create user");

            if (request.Roles != null && request.Roles.Any())
            {
                var validRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                var invalidRoles = request.Roles.Except(validRoles).ToList();
                if (invalidRoles.Any())
                    throw new ApiException($"Invalid roles: {string.Join(", ", invalidRoles)}");

                var roleResult = await _userManager.AddToRolesAsync(user, request.Roles);
                if (!roleResult.Succeeded)
                    throw new ApiException("Failed to assign roles to user");
            }

            return new Response<string>(user.Id, "User created successfully");
        }

        public async Task<Response<string>> UpdateUserAsync(string userId, UpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ApiException("User not found");

            var validationErrors = new List<string>();

            if (!string.Equals(user.Email, request.Email, System.StringComparison.OrdinalIgnoreCase))
            {
                var existing = await _userManager.FindByEmailAsync(request.Email);
                if (existing != null && existing.Id != user.Id)
                    validationErrors.Add("This email address is already taken. Please try another.");
            }

            if (!string.Equals(user.UserName, request.UserName, System.StringComparison.OrdinalIgnoreCase))
            {
                var existing = await _userManager.FindByNameAsync(request.UserName);
                if (existing != null && existing.Id != user.Id)
                    validationErrors.Add("User already exists. Please enter a different username.");
            }

            if (!string.Equals(user.EmpNumber, request.EmpNumber, System.StringComparison.OrdinalIgnoreCase))
            {
                var existing = await _userManager.Users
                                .Where(u => u.EmpNumber == request.EmpNumber)
                                .FirstOrDefaultAsync(); 
                if (existing != null && existing.Id != user.Id)
                    validationErrors.Add("This employee number is already taken. Please try another.");
            }

            if (validationErrors.Any())
                throw new ValidationException(validationErrors.Select(e => new FluentValidation.Results.ValidationFailure("", e)));

            // Update
            user.UserName = request.UserName ?? user.UserName;
            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.Email = request.Email ?? user.Email;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            user.EmpNumber = request.EmpNumber ?? user.EmpNumber;

            // Update user in the database
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new ApiException("Failed to update user details");

            // Update roles if provided
            if (request.Roles != null && request.Roles.Any())
            {
                var existingRoles = await _userManager.GetRolesAsync(user);
                var validRoles = _roleManager.Roles.Select(r => r.Name).ToList();

                // Validate if all roles in request exist in the system
                var invalidRoles = request.Roles.Except(validRoles).ToList();
                if (invalidRoles.Any())
                    throw new ApiException($"Invalid roles: {string.Join(", ", invalidRoles)}");

                // Remove old roles and add new ones
                await _userManager.RemoveFromRolesAsync(user, existingRoles);
                await _userManager.AddToRolesAsync(user, request.Roles);
            }

            return new Response<string>(user.Id, "User updated successfully");
        }

        public async Task<Response<string>> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ApiException("User not found");

            user.IsDeleted = true;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ApiException("Failed to delete user");

            return new Response<string>(userId, "User deleted successfully");
        }

        public async Task<Response<string>> ReactivateUserAsync(string userId, ReactivateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsDeleted)
                throw new ApiException("User not found or not deleted.");

            // Force keep old Data
            user.PhoneNumber = request.PhoneNumber;
            user.IsFirstLogin = true;
            user.IsDeleted = false;
            user.EmailConfirmed = true;

            // Optional: Reset Password
            if (!string.IsNullOrEmpty(request.TempPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, request.TempPassword);
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new ApiException("Failed to reactivate user.");

            // Update roles if needed
            if (request.Roles != null && request.Roles.Any())
            {
                var existingRoles = await _userManager.GetRolesAsync(user);
                var validRoles = _roleManager.Roles.Select(r => r.Name).ToList();

                // Validate if all roles in request exist in the system
                var invalidRoles = request.Roles.Except(validRoles).ToList();
                if (invalidRoles.Any())
                    throw new ApiException($"Invalid roles: {string.Join(", ", invalidRoles)}");

                // Remove old roles and add new ones
                await _userManager.RemoveFromRolesAsync(user, existingRoles);
                await _userManager.AddToRolesAsync(user, request.Roles);
            }

            return new Response<string>(user.Id, "User reactivated successfully");
        }

    }
}
