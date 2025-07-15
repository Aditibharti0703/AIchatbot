using AIchatbot.Interfaces.Brokers;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace AIchatbot.Brokers
{
    public class AIBroker : IAIBroker
    {
        private readonly ILogger<AIBroker> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AIBroker(ILogger<AIBroker> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<AIAnalysisResult> AnalyzeMessageAsync(string message)
        {
            return new AIAnalysisResult
            {
                Intent = "general",
                Confidence = 1.0,
                SuggestedResponse = null,
                Keywords = null
            };
        }

        public async Task<string> GenerateResponseAsync(string message, string context)
        {
            try
            {
                var apiKey = _configuration["AI:Gemini:ApiKey"];
                var model = _configuration["AI:Gemini:Model"] ?? "gemini-pro";
                var httpClient = _httpClientFactory.CreateClient();

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var requestBody = new
                {
                    contents = new[]
                    {
                        new { role = "user", parts = new[] { new { text = !string.IsNullOrEmpty(context) ? $"{context}\n{message}" : message } } }
                    }
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseString);

                // Gemini's response structure
                return result.candidates[0].content.parts[0].text.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Google Gemini API");
                return "I'm sorry, I couldn't process your request right now.";
            }
        }

        public Task<bool> IsRelevantToFAQAsync(string message, string faqQuestion)
        {
            return Task.FromResult(false);
        }

        public Task<double> CalculateSimilarityAsync(string text1, string text2)
        {
            return Task.FromResult(0.0);
        }
    }
}