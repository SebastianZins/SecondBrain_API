using Newtonsoft.Json;

namespace SecondBrain.Models.DTOs.FileSection.ChecklistSection
{
    public class ChecklistSectionUpdateRequestDTO : FileSectionDataUpdateRequestDTO
    {
        [JsonProperty("items")]
        public List<ChecklistItemDTO> Items { get; set; } = new List<ChecklistItemDTO>();
    }
}
