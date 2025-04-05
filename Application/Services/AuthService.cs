using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs.Auth;
using Application.DTOs.Result;
using Application.Interfaces;
using Application.Store;
using Application.Utilities;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly InventoryContext _context;
        private readonly IConfiguration _configuration;


        public AuthService(InventoryContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Result<TokenResponseDto>> LoginAsync(LoginRequestDto request)
        {
            var result = new Result<TokenResponseDto>(null);
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    result.StatusCode = StatusCode.BadRequest;
                    result.ErrorMessages = new List<string> { "Email and Password are required." };
                    return result;
                }

                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null || !user.Active)
                {
                    result.StatusCode = StatusCode.Unauthenticated;
                    result.ErrorMessages = new List<string> { "Invalid credentials or inactive user." };
                    return result;
                }

                if (user.PasswordHash != PasswordHasher.HashPassword(request.Password))
                {
                    result.StatusCode = StatusCode.Unauthenticated;
                    result.ErrorMessages = new List<string> { "Invalid credentials." };
                    return result;
                }

                var accessToken = GenerateAccessToken(user);
                var refreshToken = GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
                await _context.SaveChangesAsync();

                var tokenResponse = new TokenResponseDto
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    Role = user.Role?.Name
                };

                result.Data = tokenResponse;
                result.StatusCode = StatusCode.Success;
                result.SuccessMessege = "Login successful.";
                Log.Information( "logged in {FullName}", user.FullName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred during LoginAsync for email {Email}", request.Email);
                result.StatusCode = StatusCode.InternalError;
                result.ErrorMessages = new List<string> { "An error occurred during login. Please try again later." };
            }

            return result;
        }

        public async Task<Result<TokenResponseDto>> RefreshAsync(RefreshRequestDto request)
        {
            var result = new Result<TokenResponseDto>(null);
            try
            {
                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    result.StatusCode = StatusCode.BadRequest;
                    result.ErrorMessages = new List<string> { "Refresh token is required." };
                    return result;
                }

                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

                if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
                {
                    result.StatusCode = StatusCode.Unauthenticated;
                    result.ErrorMessages = new List<string> { "Invalid or expired refresh token." };
                    return result;
                }

                var newAccessToken = GenerateAccessToken(user);

                await _context.SaveChangesAsync();

                var tokenResponse = new TokenResponseDto
                {
                    Token = newAccessToken,
                };

                result.Data = tokenResponse;
                result.StatusCode = StatusCode.Success;
                result.SuccessMessege = "Token refreshed successfully.";

                Log.Information("{FullName} refreshed his token", user.FullName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred during RefreshAsync for refresh token {RefreshToken}", request.RefreshToken);
                result.StatusCode = StatusCode.InternalError;
                result.ErrorMessages = new List<string> { "An error occurred while refreshing token. Please try again later." };
            }
            return result;
        }

        private string GenerateAccessToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryMinutes = int.Parse(jwtSettings["ExpiryInMinutes"]);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("role", user.Role?.Name ?? "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
