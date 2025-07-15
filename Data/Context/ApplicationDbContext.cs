using Microsoft.EntityFrameworkCore;
using AIchatbot.Models.Entities;

namespace AIchatbot.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
            });
            
            // FAQ configuration
            modelBuilder.Entity<FAQ>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Question).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Answer).IsRequired();
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.Tags).HasMaxLength(50);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.IsActive);
            });
            
            // ChatSession configuration
            modelBuilder.Entity<ChatSession>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SessionId).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.SessionId).IsUnique();
                entity.HasIndex(e => e.UserId);
                
                // Relationship with User
                entity.HasOne(e => e.User)
                      .WithMany(u => u.ChatSessions)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            // ChatMessage configuration
            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Intent).HasMaxLength(100);
                entity.Property(e => e.Confidence).HasMaxLength(10);
                entity.Property(e => e.ResponseSource).HasMaxLength(20);
                entity.HasIndex(e => e.ChatSessionId);
                entity.HasIndex(e => e.Timestamp);
                
                // Relationship with ChatSession
                entity.HasOne(e => e.ChatSession)
                      .WithMany(cs => cs.Messages)
                      .HasForeignKey(e => e.ChatSessionId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Relationship with FAQ
                entity.HasOne(e => e.RelatedFAQ)
                      .WithMany()
                      .HasForeignKey(e => e.RelatedFAQId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
            
            // Seed data for FAQs
            SeedFAQData(modelBuilder);
        }
        
        private void SeedFAQData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FAQ>().HasData(
                new FAQ
                {
                    Id = 1,
                    Question = "How can I track my order?",
                    Answer = "You can track your order by logging into your account and visiting the 'Order History' section, or by using the tracking number provided in your order confirmation email.",
                    Category = "Order Tracking",
                    Tags = "order,tracking,delivery",
                    Priority = 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new FAQ
                {
                    Id = 2,
                    Question = "What is your return policy?",
                    Answer = "We offer a 30-day return policy for most items. Products must be in original condition with all tags attached. Some items may have different return policies.",
                    Category = "Returns",
                    Tags = "return,refund,policy",
                    Priority = 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new FAQ
                {
                    Id = 3,
                    Question = "How long does delivery take?",
                    Answer = "Standard delivery takes 3-5 business days. Express delivery (1-2 business days) is available for an additional fee. International shipping may take 7-14 business days.",
                    Category = "Delivery",
                    Tags = "delivery,shipping,time",
                    Priority = 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new FAQ
                {
                    Id = 4,
                    Question = "What payment methods do you accept?",
                    Answer = "We accept all major credit cards (Visa, MasterCard, American Express), PayPal, Apple Pay, Google Pay, and bank transfers.",
                    Category = "Payment",
                    Tags = "payment,credit card,paypal",
                    Priority = 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new FAQ
                {
                    Id = 5,
                    Question = "Can I cancel my order?",
                    Answer = "Orders can be cancelled within 1 hour of placement if they haven't been processed for shipping. Contact our customer service team immediately for assistance.",
                    Category = "Orders",
                    Tags = "cancel,order,modification",
                    Priority = 2,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
} 