using System.ComponentModel.DataAnnotations;

namespace AIchatbot.Models.Entities
{
    public class FAQ
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Question { get; set; } = string.Empty;
        
        [Required]
        public string Answer { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? Category { get; set; }
        
        [StringLength(50)]
        public string? Tags { get; set; }
        
        public int Priority { get; set; } = 1;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public int ViewCount { get; set; } = 0;
    }
} 