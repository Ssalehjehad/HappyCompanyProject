using Application.DTOs.Result;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Store;
using Application.Utilities;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly InventoryContext _context;

        public UserService(InventoryContext context)
        {
            _context = context;
        }

        public async Task<Result<UserDto>> GetUserAsync(int id)
        {
            var result = new Result<UserDto>(null);
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                result.StatusCode = StatusCode.NotFound;
                result.ErrorMessages = new List<string> { "User not found." };
                return result;
            }
            result.Data = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name,
                Active = user.Active
            };
            result.StatusCode = StatusCode.Success;
            return result;
        }

        public async Task<Result<List<UserDto>>> GetAllUsersAsync()
        {
            var result = new Result<List<UserDto>>(null);
            var users = await _context.Users.Include(u => u.Role).ToListAsync();
            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name,
                Active = user.Active
            }).ToList();
            result.Data = userDtos;
            result.StatusCode = StatusCode.Success;
            return result;
        }

        public async Task<Result<UserDto>> CreateUserAsync(CreateUserDto dto)
        {
            var result = new Result<UserDto>(null);
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                result.StatusCode = StatusCode.AlreadyExist;
                result.ErrorMessages = new List<string> { "A user with this email already exists." };
                return result;
            }

            var newUser = new User
            {
                Email = dto.Email,
                FullName = dto.FullName,
                RoleId = dto.RoleId,
                Active = dto.Active,
                PasswordHash = PasswordHasher.HashPassword(dto.Password)
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            result.Data = new UserDto
            {
                Id = newUser.Id,
                Email = newUser.Email,
                FullName = newUser.FullName,
                RoleId = newUser.RoleId,
                RoleName = (await _context.Roles.FindAsync(newUser.RoleId))?.Name,
                Active = newUser.Active
            };
            result.StatusCode = StatusCode.Success;
            result.SuccessMessege = "User created successfully.";
            return result;
        }

        public async Task<Result<UserDto>> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var result = new Result<UserDto>(null);
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                result.StatusCode = StatusCode.NotFound;
                result.ErrorMessages = new List<string> { "User not found." };
                return result;
            }

            user.Email = dto.Email;
            user.FullName = dto.FullName;
            user.RoleId = dto.RoleId;
            user.Active = dto.Active;

            await _context.SaveChangesAsync();

            result.Data = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                RoleId = user.RoleId,
                RoleName = (await _context.Roles.FindAsync(user.RoleId))?.Name,
                Active = user.Active
            };
            result.StatusCode = StatusCode.Success;
            result.SuccessMessege = "User updated successfully.";
            return result;
        }

        public async Task<Result<bool>> DeleteUserAsync(int id)
        {
            var result = new Result<bool>(false);
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                result.StatusCode = StatusCode.NotFound;
                result.ErrorMessages = new List<string> { "User not found." };
                return result;
            }

            if (user.Role?.Name == "Admin")
            {
                result.StatusCode = StatusCode.BadRequest;
                result.ErrorMessages = new List<string> { "Admin user cannot be deleted." };
                return result;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            result.Data = true;
            result.StatusCode = StatusCode.Success;
            result.SuccessMessege = "User deleted successfully.";
            return result;
        }

        public async Task<Result<bool>> ChangePasswordAsync(int id, ChangePasswordDto dto)
        {
            var result = new Result<bool>(false);
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                result.StatusCode = StatusCode.NotFound;
                result.ErrorMessages = new List<string> { "User not found." };
                return result;
            }

            user.PasswordHash = PasswordHasher.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            result.Data = true;
            result.StatusCode = StatusCode.Success;
            result.SuccessMessege = "Password changed successfully.";
            return result;
        }
    }
}
