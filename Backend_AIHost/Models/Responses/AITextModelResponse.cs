using System.Text.Json.Serialization;

namespace Backend_AIHost.Models.Responses
{
    public class AITextModelResponse
    {
        [JsonPropertyName("generated_text")]
        public string GeneratedText { get; set; }
    }
}
