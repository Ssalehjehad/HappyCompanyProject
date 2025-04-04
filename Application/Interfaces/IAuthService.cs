using Application.DTOs.Auth;
using Application.DTOs.Result;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<TokenResponseDto>> LoginAsync(LoginRequestDto request);
        Task<Result<TokenResponseDto>> RefreshAsync(RefreshRequestDto request);
    }
}
