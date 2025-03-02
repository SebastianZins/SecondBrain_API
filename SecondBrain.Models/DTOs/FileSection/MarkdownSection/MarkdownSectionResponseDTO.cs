using Newtonsoft.Json;
using SecondBrain.Models.DatabaseModels.MongoDB.Section;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Models.DTOs.FileSection.MarkdownSection
{
    public class MarkdownSectionResponseDTO : FileSectionResponseDTO
    {
        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;

        public MarkdownSectionResponseDTO(FileSectionNode metaData, MarkdownSectionModel data) : base(metaData)
        {
            Text = data.text;
        }
    }
}