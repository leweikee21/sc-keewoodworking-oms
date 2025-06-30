using Application.DTOs.Account;
using Application.DTOs.User;
using Application.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<List<string>> GetAllUserNamesAsync();
        Task<PagedResponse<List<UserDto>>> GetAllUsersAsync(int pageNumber, int pageSize, string? search = null, string? role = null);
        Task<Response<UserDto>> GetUserByIdAsync(string userId);
        Task<List<Response<UserDto>>> GetUsersByRoleAsync(string role);
        Task<Response<string>> CreateUserAsync(RegisterRequest request);
        Task<Response<string>> UpdateUserAsync(string userId, UpdateUserRequest request);
        Task<Response<string>> DeleteUserAsync(string userId);
        Task<Response<string>> ReactivateUserAsync(string userId, ReactivateUserRequest request);
    }
}