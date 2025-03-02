using Newtonsoft.Json;

namespace SecondBrain.Models.DTOs.FileSection.TextSection
{
    public class TextSectionUpdateRequestDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;
    }
}
