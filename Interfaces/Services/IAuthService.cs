using AIchatbot.Models.DTOs;

namespace AIchatbot.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<bool> ValidateTokenAsync(string token);
        Task<string?> GetUserIdFromTokenAsync(string token);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(string email);
    }
} 