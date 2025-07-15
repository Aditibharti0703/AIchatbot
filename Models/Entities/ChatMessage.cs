using System.ComponentModel.DataAnnotations;

namespace AIchatbot.Models.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        
        public int ChatSessionId { get; set; }
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public bool IsFromUser { get; set; }
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        public string? Intent { get; set; }
        
        public string? Confidence { get; set; }
        
        public string? ResponseSource { get; set; } // "FAQ", "AI", "Fallback"
        
        public int? RelatedFAQId { get; set; }
        
        // Navigation properties
        public virtual ChatSession ChatSession { get; set; } = null!;
        public virtual FAQ? RelatedFAQ { get; set; }
    }
} 