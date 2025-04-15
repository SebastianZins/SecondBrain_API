using Newtonsoft.Json;

namespace SecondBrain.Models.DTOs.FileSection.ChecklistSection
{
    public class ChecklistSectionDeleteRequestDTO
    {
        [JsonProperty("sectionId")]
        public Guid SectionId { get; set; } = Guid.Empty;

        [JsonProperty("indices")]
        public List<int> Indices { get; set; } = new List<int>();
    }
}
