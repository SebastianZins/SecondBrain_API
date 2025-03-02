using Newtonsoft.Json;

namespace SecondBrain.Models.DTOs.FileSection.ListSection
{
    public class ListSectionDeleteRequestDTO
    {
        [JsonProperty("sectionId")]
        public Guid SectionId { get; set; }  = Guid.Empty;

        [JsonProperty("indices")]
        public List<int> Indices { get; set; } = new List<int>();
    }
}
