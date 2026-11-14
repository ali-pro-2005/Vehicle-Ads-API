using FinalProjectWebAPI.DTOs;

namespace FinalProjectWebAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);
        Task<bool> DeleteAccountAsync(int userId, string password);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto);
    }
}