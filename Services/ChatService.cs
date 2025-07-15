using Microsoft.EntityFrameworkCore;
using AIchatbot.Data.Context;
using AIchatbot.Interfaces.Services;
using AIchatbot.Interfaces.Brokers;
using AIchatbot.Models.DTOs;
using AIchatbot.Models.Entities;
using AIchatbot.Models.Responses;

namespace AIchatbot.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAIBroker _aiBroker;
        private readonly ILogger<ChatService> _logger;
        
        public ChatService(ApplicationDbContext context, IAIBroker aiBroker, ILogger<ChatService> logger)
        {
            _context = context;
            _aiBroker = aiBroker;
            _logger = logger;
        }
        
        public async Task<ChatResponse> ProcessMessageAsync(ChatRequest request, int userId)
        {
            try
            {
                _logger.LogInformation("Processing chat message for user {UserId}: {Message}", userId, request.Message);
                
                // Get or create chat session
                var session = await GetOrCreateChatSessionAsync(request.SessionId, userId);
                
                // Save user message
                var userMessage = new ChatMessage
                {
                    ChatSessionId = session.Id,
                    Content = request.Message,
                    IsFromUser = true,
                    Timestamp = DateTime.UtcNow
                };
                
                _context.ChatMessages.Add(userMessage);
                await _context.SaveChangesAsync();
                
                // Always use OpenAI for response
                var context = await GetChatContextAsync(session.Id);
                var aiResponse = await _aiBroker.GenerateResponseAsync(request.Message, context);
                
                // Save bot response
                var botMessage = new ChatMessage
                {
                    ChatSessionId = session.Id,
                    Content = aiResponse,
                    IsFromUser = false,
                    Timestamp = DateTime.UtcNow,
                    Intent = "general",
                    Confidence = "1.0",
                    ResponseSource = "AI",
                    RelatedFAQId = null
                };
                
                _context.ChatMessages.Add(botMessage);
                await _context.SaveChangesAsync();
                
                return new ChatResponse
                {
                    Success = true,
                    Message = aiResponse,
                    SessionId = session.SessionId,
                    Intent = "general",
                    Confidence = 1.0,
                    ResponseSource = "AI",
                    RelatedFAQId = null,
                    Suggestions = new List<string>(),
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message for user {UserId}", userId);
                return new ChatResponse
                {
                    Success = false,
                    Message = "I'm sorry, I'm having trouble processing your request right now. Please try again later.",
                    Timestamp = DateTime.UtcNow
                };
            }
        }
        
        public async Task<ChatSessionDto> GetChatSessionAsync(string sessionId, int userId)
        {
            try
            {
                var session = await _context.ChatSessions
                    .Include(cs => cs.Messages.OrderBy(m => m.Timestamp))
                    .FirstOrDefaultAsync(cs => cs.SessionId == sessionId && cs.UserId == userId);
                
                if (session == null)
                    return new ChatSessionDto();
                
                return new ChatSessionDto
                {
                    Id = session.Id,
                    SessionId = session.SessionId,
                    StartedAt = session.StartedAt,
                    EndedAt = session.EndedAt,
                    IsActive = session.IsActive,
                    Messages = session.Messages.Select(m => new ChatMessageDto
                    {
                        Id = m.Id,
                        Content = m.Content,
                        IsFromUser = m.IsFromUser,
                        Timestamp = m.Timestamp,
                        Intent = m.Intent,
                        ResponseSource = m.ResponseSource
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving chat session {SessionId} for user {UserId}", sessionId, userId);
                return new ChatSessionDto();
            }
        }
        
        public async Task<IEnumerable<ChatSessionDto>> GetUserChatSessionsAsync(int userId)
        {
            try
            {
                var sessions = await _context.ChatSessions
                    .Where(cs => cs.UserId == userId)
                    .OrderByDescending(cs => cs.StartedAt)
                    .ToListAsync();
                
                return sessions.Select(s => new ChatSessionDto
                {
                    Id = s.Id,
                    SessionId = s.SessionId,
                    StartedAt = s.StartedAt,
                    EndedAt = s.EndedAt,
                    IsActive = s.IsActive,
                    Messages = new List<ChatMessageDto>()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving chat sessions for user {UserId}", userId);
                return Enumerable.Empty<ChatSessionDto>();
            }
        }
        
        public async Task<bool> EndChatSessionAsync(string sessionId, int userId)
        {
            try
            {
                var session = await _context.ChatSessions
                    .FirstOrDefaultAsync(cs => cs.SessionId == sessionId && cs.UserId == userId);
                
                if (session == null) return false;
                
                session.IsActive = false;
                session.EndedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ending chat session {SessionId} for user {UserId}", sessionId, userId);
                return false;
            }
        }
        
        public async Task<IEnumerable<FAQDto>> GetFAQsAsync(string? category = null)
        {
            try
            {
                var query = _context.FAQs.Where(f => f.IsActive);
                
                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(f => f.Category == category);
                }
                
                var faqs = await query.OrderBy(f => f.Priority).ThenBy(f => f.Question).ToListAsync();
                
                return faqs.Select(f => new FAQDto
                {
                    Id = f.Id,
                    Question = f.Question,
                    Answer = f.Answer,
                    Category = f.Category,
                    Tags = f.Tags,
                    Priority = f.Priority,
                    ViewCount = f.ViewCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving FAQs");
                return Enumerable.Empty<FAQDto>();
            }
        }
        
        public async Task<FAQDto?> GetFAQByIdAsync(int id)
        {
            try
            {
                var faq = await _context.FAQs.FindAsync(id);
                
                if (faq == null || !faq.IsActive) return null;
                
                return new FAQDto
                {
                    Id = faq.Id,
                    Question = faq.Question,
                    Answer = faq.Answer,
                    Category = faq.Category,
                    Tags = faq.Tags,
                    Priority = faq.Priority,
                    ViewCount = faq.ViewCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving FAQ with ID {Id}", id);
                return null;
            }
        }
        
        private async Task<ChatSession> GetOrCreateChatSessionAsync(string? sessionId, int userId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                // Create new session
                var newSession = new ChatSession
                {
                    UserId = userId,
                    SessionId = Guid.NewGuid().ToString(),
                    StartedAt = DateTime.UtcNow,
                    IsActive = true
                };
                
                _context.ChatSessions.Add(newSession);
                await _context.SaveChangesAsync();
                return newSession;
            }
            
            // Get existing session
            var session = await _context.ChatSessions
                .FirstOrDefaultAsync(cs => cs.SessionId == sessionId && cs.UserId == userId);
            
            if (session == null)
            {
                // Create new session with provided ID
                session = new ChatSession
                {
                    UserId = userId,
                    SessionId = sessionId,
                    StartedAt = DateTime.UtcNow,
                    IsActive = true
                };
                
                _context.ChatSessions.Add(session);
                await _context.SaveChangesAsync();
            }
            
            return session;
        }
        
        private async Task<string> GetChatContextAsync(int sessionId)
        {
            try
            {
                var recentMessages = await _context.ChatMessages
                    .Where(m => m.ChatSessionId == sessionId)
                    .OrderByDescending(m => m.Timestamp)
                    .Take(5)
                    .OrderBy(m => m.Timestamp)
                    .ToListAsync();
                
                return string.Join(" ", recentMessages.Select(m => m.Content));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chat context for session {SessionId}", sessionId);
                return string.Empty;
            }
        }
    }
} 