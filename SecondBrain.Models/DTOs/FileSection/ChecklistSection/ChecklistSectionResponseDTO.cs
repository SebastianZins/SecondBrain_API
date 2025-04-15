using Newtonsoft.Json;
using SecondBrain.Models.DatabaseModels.MongoDB.File;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Models.DTOs.FileSection.ChecklistSection
{
    public class ChecklistSectionResponseDTO : FileSectionResponseDTO
    {
        [JsonProperty("items")]
        public List<ChecklistItemDTO> Items { get; set; } = new List<ChecklistItemDTO>();

        public ChecklistSectionResponseDTO(FileSectionNode metaData, ChecklistSectionModel data) : base(metaData)
        {
            Items = data.items.Select(i => new ChecklistItemDTO(i)).ToList();
        }
    }
}
