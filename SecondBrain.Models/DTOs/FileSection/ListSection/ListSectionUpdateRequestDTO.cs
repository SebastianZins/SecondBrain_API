using Newtonsoft.Json;

namespace SecondBrain.Models.DTOs.FileSection.ListSelection
{
    public class ListSectionUpdateRequestDTO : FileSectionDataUpdateRequestDTO
    {
        [JsonProperty("items")]
        public List<string> Items { get; set; } = new List<string>();
    }
}
