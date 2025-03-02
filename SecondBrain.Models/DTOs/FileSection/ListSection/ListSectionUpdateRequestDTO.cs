using Newtonsoft.Json;

namespace SecondBrain.Models.DTOs.FileSection.ListSelection
{
    public class ListSectionUpdateRequestDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("items")]
        public List<string> Items { get; set; } = new List<string>();
    }
}
