using Application.DTOs.Result;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Store;
using Application.Utilities;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
            try
            {
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
                Log.Information("retrived {Email}", user.Email);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting user with id {Id}", id);
                result.StatusCode = StatusCode.InternalError;
                result.ErrorMessages = new List<string> { "An error occurred while retrieving the user." };
            }
            return result;
        }

        public async Task<Result<List<UserDto>>> GetAllUsersAsync()
        {
            var result = new Result<List<UserDto>>(null);
            try
            {
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
                Log.Information("retrived all users");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting all users");
                result.StatusCode = StatusCode.InternalError;
                result.ErrorMessages = new List<string> { "An error occurred while retrieving users." };
            }
            return result;
        }

        public async Task<Result<UserDto>> CreateUserAsync(CreateUserDto dto)
        {
            var result = new Result<UserDto>(null);
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                {
                    result.StatusCode = StatusCode.AlreadyExist;
                    result.ErrorMessages = new List<string> { "A user with this email already exists." };
                    Log.Information("Creation of user with duplicate {Email}", dto.Email);
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
                Log.Information("Creation of user with {Email}", dto.Email);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating user with email {Email}", dto.Email);
                result.StatusCode = StatusCode.InternalError;
                result.ErrorMessages = new List<string> { "An error occurred while creating the user." };
            }
            return result;
        }

        public async Task<Result<UserDto>> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var result = new Result<UserDto>(null);
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    result.StatusCode = StatusCode.NotFound;
                    result.ErrorMessages = new List<string> { "User not found." };
                    Log.Information("User not found for update operation {Email}", dto.Email);

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
                Log.Information("User updated successfully, old email {Email}", dto.Email);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating user with id {Id}", id);
                result.StatusCode = StatusCode.InternalError;
                result.ErrorMessages = new List<string> { "An error occurred while updating the user." };
            }
            return result;
        }

        public async Task<Result<bool>> DeleteUserAsync(int id)
        {
            var result = new Result<bool>(false);
            try
            {
                var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    result.StatusCode = StatusCode.NotFound;
                    result.ErrorMessages = new List<string> { "User not found." };
                    Log.Information("User not found for delete operation {id}", id);
                    return result;
                }

                if (user.Role?.Name == "Admin")
                {
                    result.StatusCode = StatusCode.BadRequest;
                    result.ErrorMessages = new List<string> { "Admin user cannot be deleted." };
                    Log.Information("Admin user cannot be deleted {Email}", user.Email);

                    return result;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                result.Data = true;
                result.StatusCode = StatusCode.Success;
                result.SuccessMessege = "User deleted successfully.";
                Log.Information("User deleted successfully {Email}", user.Email);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting user with id {Id}", id);
                result.StatusCode = StatusCode.InternalError;
                result.ErrorMessages = new List<string> { "An error occurred while deleting the user." };
            }
            return result;
        }

        public async Task<Result<bool>> ChangePasswordAsync(int id, ChangePasswordDto dto)
        {
            var result = new Result<bool>(false);
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    result.StatusCode = StatusCode.NotFound;
                    result.ErrorMessages = new List<string> { "User not found." };
                    Log.Information("User not found for password changing operation {id}", id);

                    return result;
                }

                user.PasswordHash = PasswordHasher.HashPassword(dto.NewPassword);
                await _context.SaveChangesAsync();

                result.Data = true;
                result.StatusCode = StatusCode.Success;
                result.SuccessMessege = "Password changed successfully.";
                Log.Information("Password changed successfully for {Email}", user.Email);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error changing password for user with id {Id}", id);
                result.StatusCode = StatusCode.InternalError;
                result.ErrorMessages = new List<string> { "An error occurred while changing the password." };
            }
            return result;
        }
    }
}
