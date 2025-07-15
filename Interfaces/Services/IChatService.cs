using AIchatbot.Models.DTOs;

namespace AIchatbot.Interfaces.Services
{
    public interface IChatService
    {
        Task<ChatResponse> ProcessMessageAsync(ChatRequest request, int userId);
        Task<ChatSessionDto> GetChatSessionAsync(string sessionId, int userId);
        Task<IEnumerable<ChatSessionDto>> GetUserChatSessionsAsync(int userId);
        Task<bool> EndChatSessionAsync(string sessionId, int userId);
        Task<IEnumerable<FAQDto>> GetFAQsAsync(string? category = null);
        Task<FAQDto?> GetFAQByIdAsync(int id);
    }
} 