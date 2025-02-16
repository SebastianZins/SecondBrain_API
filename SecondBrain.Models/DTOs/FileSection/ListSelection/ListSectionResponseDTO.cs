using Newtonsoft.Json;
using SecondBrain.Models.DatabaseModels.MongoDB.Section;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Models.DTOs.FileSection.ListSelection
{
    public class ListSectionResponseDTO : FileSectionResponseDTO
    {
        [JsonProperty("items")]
        public List<string> Items = new List<string>();

        public ListSectionResponseDTO(FileSectionNode metaData, ListSectionModel data) : base(metaData)
        {
            Items = data.items;
        }
    }
}
