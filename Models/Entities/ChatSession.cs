using System.ComponentModel.DataAnnotations;

namespace AIchatbot.Models.Entities
{
    public class ChatSession
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string SessionId { get; set; } = string.Empty;
        
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? EndedAt { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
} 