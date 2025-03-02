using Newtonsoft.Json;
using SecondBrain.Models.DatabaseModels.MongoDB.Section;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Models.DTOs.FileSection.TextSection
{
    public class TextSectionResponseDTO : FileSectionResponseDTO
    {
        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;

        public TextSectionResponseDTO(FileSectionNode metaData, TextSectionModel data) : base(metaData)
        {
            Text = data.text;
        }
    }
}
