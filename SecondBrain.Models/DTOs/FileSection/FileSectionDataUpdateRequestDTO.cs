using Newtonsoft.Json;

namespace SecondBrain.Models.DTOs.FileSection
{
    public class FileSectionDataUpdateRequestDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("tags")]
        public List<string> Tags { get; set; } = new List<string>();
    }
}
