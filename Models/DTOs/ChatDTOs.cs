using System.ComponentModel.DataAnnotations;

namespace AIchatbot.Models.DTOs
{
    public class ChatRequest
    {
        [Required]
        public string Message { get; set; } = string.Empty;
        
        public string? SessionId { get; set; }
        
        public string? Context { get; set; }
    }
    
    public class ChatResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? SessionId { get; set; }
        public string? Intent { get; set; }
        public double? Confidence { get; set; }
        public string? ResponseSource { get; set; }
        public int? RelatedFAQId { get; set; }
        public List<string>? Suggestions { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
    
    public class ChatSessionDto
    {
        public int Id { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public bool IsActive { get; set; }
        public List<ChatMessageDto> Messages { get; set; } = new List<ChatMessageDto>();
    }
    
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsFromUser { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Intent { get; set; }
        public string? ResponseSource { get; set; }
    }
    
    public class FAQDto
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Tags { get; set; }
        public int Priority { get; set; }
        public int ViewCount { get; set; }
    }
} 