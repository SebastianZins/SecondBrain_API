using Newtonsoft.Json;

namespace SecondBrain.Models.DTOs.FileSection.TextSection
{
    public class TextSectionUpdateRequestDTO : FileSectionDataUpdateRequestDTO
    {
        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;
    }
}
