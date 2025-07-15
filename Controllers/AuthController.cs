using Microsoft.AspNetCore.Mvc;
using AIchatbot.Interfaces.Services;
using AIchatbot.Models.DTOs;
using AIchatbot.Models.Responses;

namespace AIchatbot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        
        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="request">Registration request</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(ApiResponse.ErrorResult("Validation failed", errors));
                }
                
                var result = await _authService.RegisterAsync(request);
                
                if (result.Success)
                {
                    return Ok(ApiResponse<AuthResponse>.SuccessResult(result, "User registered successfully"));
                }
                
                return BadRequest(ApiResponse.ErrorResult(result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return StatusCode(500, ApiResponse.ErrorResult("An error occurred during registration"));
            }
        }
        
        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="request">Login request</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(ApiResponse.ErrorResult("Validation failed", errors));
                }
                
                var result = await _authService.LoginAsync(request);
                
                if (result.Success)
                {
                    return Ok(ApiResponse<AuthResponse>.SuccessResult(result, "Login successful"));
                }
                
                return BadRequest(ApiResponse.ErrorResult(result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return StatusCode(500, ApiResponse.ErrorResult("An error occurred during login"));
            }
        }
        
        /// <summary>
        /// Validate JWT token
        /// </summary>
        /// <param name="token">JWT token to validate</param>
        /// <returns>Validation result</returns>
        [HttpPost("validate-token")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> ValidateToken([FromBody] string token)
        {
            try
            {
                var isValid = await _authService.ValidateTokenAsync(token);
                
                return Ok(ApiResponse<bool>.SuccessResult(isValid, "Token validation completed"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return StatusCode(500, ApiResponse.ErrorResult("An error occurred during token validation"));
            }
        }
        
        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="request">Password change request</param>
        /// <returns>Success status</returns>
        [HttpPost("change-password")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(ApiResponse.ErrorResult("Validation failed", errors));
                }
                
                // Extract user ID from JWT token (in a real app, you'd get this from the token)
                var userId = 1; // This should come from the authenticated user context
                
                var success = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
                
                if (success)
                {
                    return Ok(ApiResponse.SuccessResult("Password changed successfully"));
                }
                
                return BadRequest(ApiResponse.ErrorResult("Invalid current password"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, ApiResponse.ErrorResult("An error occurred while changing password"));
            }
        }
    }
    
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
} 