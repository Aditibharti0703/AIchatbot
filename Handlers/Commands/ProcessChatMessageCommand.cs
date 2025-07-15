using AIchatbot.Models.DTOs;
using AIchatbot.Models.Responses;
using AIchatbot.Interfaces.Handlers;
using AIchatbot.Interfaces.Services;

namespace AIchatbot.Handlers.Commands
{
    public class ProcessChatMessageCommand : ICommand
    {
        public ChatRequest Request { get; set; }
        public int UserId { get; set; }
        
        public ProcessChatMessageCommand(ChatRequest request, int userId)
        {
            Request = request;
            UserId = userId;
        }
    }
    
    public class ProcessChatMessageCommandHandler : ICommandHandler<ProcessChatMessageCommand, ChatResponse>
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ProcessChatMessageCommandHandler> _logger;
        
        public ProcessChatMessageCommandHandler(IChatService chatService, ILogger<ProcessChatMessageCommandHandler> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }
        
        public async Task<ChatResponse> HandleAsync(ProcessChatMessageCommand command)
        {
            try
            {
                _logger.LogInformation("Processing chat message for user {UserId}", command.UserId);
                
                var response = await _chatService.ProcessMessageAsync(command.Request, command.UserId);
                
                _logger.LogInformation("Chat message processed successfully for user {UserId}", command.UserId);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message for user {UserId}", command.UserId);
                return new ChatResponse
                {
                    Success = false,
                    Message = "An error occurred while processing your message. Please try again.",
                    Timestamp = DateTime.UtcNow
                };
            }
        }
    }
} 