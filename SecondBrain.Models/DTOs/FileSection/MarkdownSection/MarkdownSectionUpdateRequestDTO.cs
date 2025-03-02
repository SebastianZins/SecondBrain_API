using Newtonsoft.Json;

namespace SecondBrain.Models.DTOs.FileSection.MarkdownSection
{
    public class MarkdownSectionUpdateRequestDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;
    }
}
