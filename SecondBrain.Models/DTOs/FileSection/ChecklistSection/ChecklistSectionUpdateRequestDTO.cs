using Newtonsoft.Json;

namespace SecondBrain.Models.DTOs.FileSection.ChecklistSection
{
    public class ChecklistSectionUpdateRequestDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("items")]
        public List<ChecklistItemResponseDTO> Items { get; set; } = new List<ChecklistItemResponseDTO>();
    }
}
