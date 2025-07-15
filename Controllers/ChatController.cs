using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AIchatbot.Interfaces.Services;
using AIchatbot.Models.DTOs;
using AIchatbot.Models.Responses;
using AIchatbot.Handlers.Commands;
using AIchatbot.Handlers.Queries;
using AIchatbot.Interfaces.Handlers;

namespace AIchatbot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ICommandHandler<ProcessChatMessageCommand, ChatResponse> _processMessageHandler;
        private readonly IQueryHandler<GetFAQsQuery, IEnumerable<FAQDto>> _getFAQsHandler;
        private readonly ILogger<ChatController> _logger;
        
        public ChatController(
            IChatService chatService,
            ICommandHandler<ProcessChatMessageCommand, ChatResponse> processMessageHandler,
            IQueryHandler<GetFAQsQuery, IEnumerable<FAQDto>> getFAQsHandler,
            ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _processMessageHandler = processMessageHandler;
            _getFAQsHandler = getFAQsHandler;
            _logger = logger;
        }
        
        /// <summary>
        /// Send a message to the chatbot
        /// </summary>
        /// <param name="request">Chat message request</param>
        /// <returns>Chatbot response</returns>
        [HttpPost("send")]
        [ProducesResponseType(typeof(ApiResponse<ChatResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
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
                
                // Using CQRS pattern with command handler
                var command = new ProcessChatMessageCommand(request, userId);
                var response = await _processMessageHandler.HandleAsync(command);
                
                if (response.Success)
                {
                    return Ok(ApiResponse<ChatResponse>.SuccessResult(response, "Message processed successfully"));
                }
                
                return BadRequest(ApiResponse.ErrorResult(response.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return StatusCode(500, ApiResponse.ErrorResult("An error occurred while processing your message"));
            }
        }
        
        /// <summary>
        /// Get chat session history
        /// </summary>
        /// <param name="sessionId">Session ID</param>
        /// <returns>Chat session with messages</returns>
        [HttpGet("session/{sessionId}")]
        [ProducesResponseType(typeof(ApiResponse<ChatSessionDto>), 200)]
        public async Task<IActionResult> GetChatSession(string sessionId)
        {
            try
            {
                var userId = 1; // This should come from the authenticated user context
                
                var session = await _chatService.GetChatSessionAsync(sessionId, userId);
                
                if (session.Id == 0)
                {
                    return NotFound(ApiResponse.ErrorResult("Chat session not found"));
                }
                
                return Ok(ApiResponse<ChatSessionDto>.SuccessResult(session, "Chat session retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving chat session {SessionId}", sessionId);
                return StatusCode(500, ApiResponse.ErrorResult("An error occurred while retrieving the chat session"));
            }
        }
        
        /// <summary>
        /// Get user's chat sessions
        /// </summary>
        /// <returns>List of user's chat sessions</returns>
        [HttpGet("sessions")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ChatSessionDto>>), 200)]
        public async Task<IActionResult> GetUserChatSessions()
        {
            try
            {
                var userId = 1; // This should come from the authenticated user context
                
                var sessions = await _chatService.GetUserChatSessionsAsync(userId);
                
                return Ok(ApiResponse<IEnumerable<ChatSessionDto>>.SuccessResult(sessions, "Chat sessions retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user chat sessions");
                return StatusCode(500, ApiResponse.ErrorResult("An error occurred while retrieving chat sessions"));
            }
        }
        
        /// <summary>
        /// End a chat session
        /// </summary>
        /// <param name="sessionId">Session ID to end</param>
        /// <returns>Success status</returns>
        [HttpPost("session/{sessionId}/end")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> EndChatSession(string sessionId)
        {
            try
            {
                var userId = 1; // This should come from the authenticated user context
                
                var success = await _chatService.EndChatSessionAsync(sessionId, userId);
                
                if (success)
                {
                    return Ok(ApiResponse.SuccessResult("Chat session ended successfully"));
                }
                
                return NotFound(ApiResponse.ErrorResult("Chat session not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ending chat session {SessionId}", sessionId);
                return StatusCode(500, ApiResponse.ErrorResult("An error occurred while ending the chat session"));
            }
        }
        
        /// <summary>
        /// Get all FAQs
        /// </summary>
        /// <param name="category">Optional category filter</param>
        /// <returns>List of FAQs</returns>
        [HttpGet("faqs")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<FAQDto>>), 200)]
        public async Task<IActionResult> GetFAQs([FromQuery] string? category = null)
        {
            try
            {
                // Using CQRS pattern with query handler
                var query = new GetFAQsQuery(category);
                var faqs = await _getFAQsHandler.HandleAsync(query);
                
                return Ok(ApiResponse<IEnumerable<FAQDto>>.SuccessResult(faqs, "FAQs retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving FAQs");
                return StatusCode(500, ApiResponse.ErrorResult("An error occurred while retrieving FAQs"));
            }
        }
        
        /// <summary>
        /// Get FAQ by ID
        /// </summary>
        /// <param name="id">FAQ ID</param>
        /// <returns>FAQ details</returns>
        [HttpGet("faqs/{id}")]
        [ProducesResponseType(typeof(ApiResponse<FAQDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetFAQById(int id)
        {
            try
            {
                var faq = await _chatService.GetFAQByIdAsync(id);
                
                if (faq == null)
                {
                    return NotFound(ApiResponse.ErrorResult("FAQ not found"));
                }
                
                return Ok(ApiResponse<FAQDto>.SuccessResult(faq, "FAQ retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving FAQ with ID {Id}", id);
                return StatusCode(500, ApiResponse.ErrorResult("An error occurred while retrieving the FAQ"));
            }
        }
        
        /// <summary>
        /// Get FAQ categories
        /// </summary>
        /// <returns>List of FAQ categories</returns>
        [HttpGet("faqs/categories")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<string>>), 200)]
        public async Task<IActionResult> GetFAQCategories()
        {
            try
            {
                var query = new GetFAQsQuery();
                var faqs = await _getFAQsHandler.HandleAsync(query);
                
                var categories = faqs
                    .Where(f => !string.IsNullOrEmpty(f.Category))
                    .Select(f => f.Category!)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();
                
                return Ok(ApiResponse<IEnumerable<string>>.SuccessResult(categories, "FAQ categories retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving FAQ categories");
                return StatusCode(500, ApiResponse.ErrorResult("An error occurred while retrieving FAQ categories"));
            }
        }
    }
} 