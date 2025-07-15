using AIchatbot.Models.DTOs;
using AIchatbot.Interfaces.Handlers;
using AIchatbot.Interfaces.Services;

namespace AIchatbot.Handlers.Queries
{
    public class GetFAQsQuery : IQuery<IEnumerable<FAQDto>>
    {
        public string? Category { get; set; }
        
        public GetFAQsQuery(string? category = null)
        {
            Category = category;
        }
    }
    
    public class GetFAQsQueryHandler : IQueryHandler<GetFAQsQuery, IEnumerable<FAQDto>>
    {
        private readonly IChatService _chatService;
        private readonly ILogger<GetFAQsQueryHandler> _logger;
        
        public GetFAQsQueryHandler(IChatService chatService, ILogger<GetFAQsQueryHandler> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }
        
        public async Task<IEnumerable<FAQDto>> HandleAsync(GetFAQsQuery query)
        {
            try
            {
                _logger.LogInformation("Retrieving FAQs with category: {Category}", query.Category ?? "All");
                
                var faqs = await _chatService.GetFAQsAsync(query.Category);
                
                _logger.LogInformation("Retrieved {Count} FAQs", faqs.Count());
                return faqs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving FAQs");
                return Enumerable.Empty<FAQDto>();
            }
        }
    }
} 