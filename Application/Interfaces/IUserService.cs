using Application.DTOs.Result;
using Application.DTOs.User;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserDto>> GetUserAsync(int id);
        Task<Result<List<UserDto>>> GetAllUsersAsync();
        Task<Result<UserDto>> CreateUserAsync(CreateUserDto dto);
        Task<Result<UserDto>> UpdateUserAsync(int id, UpdateUserDto dto);
        Task<Result<bool>> DeleteUserAsync(int id);
        Task<Result<bool>> ChangePasswordAsync(int id, ChangePasswordDto dto);
    }
}
