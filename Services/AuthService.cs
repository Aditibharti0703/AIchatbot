using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AIchatbot.Data.Context;
using AIchatbot.Interfaces.Services;
using AIchatbot.Models.DTOs;
using AIchatbot.Models.Entities;
using AIchatbot.Models.Responses;
using Microsoft.Extensions.Configuration;

namespace AIchatbot.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        
        public AuthService(ApplicationDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }
        
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);
                
                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }
                
                if (!VerifyPassword(request.Password, user.PasswordHash))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }
                
                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                
                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();
                
                return new AuthResponse
                {
                    Success = true,
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    Message = "Login successful.",
                    User = new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        CreatedAt = user.CreatedAt,
                        LastLoginAt = user.LastLoginAt
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
                return new AuthResponse
                {
                    Success = false,
                    Message = "An error occurred during login. Please try again."
                };
            }
        }
        
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Check if user already exists
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "A user with this email already exists."
                    };
                }
                
                if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "A user with this username already exists."
                    };
                }
                
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = HashPassword(request.Password),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                
                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();
                
                return new AuthResponse
                {
                    Success = true,
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    Message = "Registration successful.",
                    User = new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        CreatedAt = user.CreatedAt,
                        LastLoginAt = user.LastLoginAt
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
                return new AuthResponse
                {
                    Success = false,
                    Message = "An error occurred during registration. Please try again."
                };
            }
        }
        
        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"));
                
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed");
                return false;
            }
        }
        
        public async Task<string?> GetUserIdFromTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"));
                
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                
                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                return userIdClaim?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting user ID from token");
                return null;
            }
        }
        
        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;
                
                if (!VerifyPassword(currentPassword, user.PasswordHash))
                    return false;
                
                user.PasswordHash = HashPassword(newPassword);
                await _context.SaveChangesAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
                return false;
            }
        }
        
        public async Task<bool> ResetPasswordAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null) return false;
                
                // In a real application, you would send a password reset email
                // For now, we'll just log the request
                _logger.LogInformation("Password reset requested for email: {Email}", email);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing password reset for email: {Email}", email);
                return false;
            }
        }
        
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
        
        private bool VerifyPassword(string password, string hash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == hash;
        }
        
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"));
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
} 