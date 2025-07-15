namespace AIchatbot.Interfaces.Brokers
{
    public class AIAnalysisResult
    {
        public string Intent { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public string? SuggestedResponse { get; set; }
        public List<string>? Keywords { get; set; }
    }
    
    public interface IAIBroker
    {
        Task<AIAnalysisResult> AnalyzeMessageAsync(string message);
        Task<string> GenerateResponseAsync(string message, string context);
        Task<bool> IsRelevantToFAQAsync(string message, string faqQuestion);
        Task<double> CalculateSimilarityAsync(string text1, string text2);
    }
} 