using Newtonsoft.Json;
using SecondBrain.Core.Enums;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Models.DTOs.FileSection
{
    public class FileSectionCreateRequestDTO
    {
        [JsonProperty("structureId")]
        public Guid StructureId { get; set; } = Guid.Empty;

        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("subtitle")]
        public string Subtitle { get; set; } = string.Empty;

        [JsonProperty("isExpanded")]
        public bool IsExpanded { get; set; } = true;

        [JsonProperty("isVisible")]
        public bool IsVisible { get; set; } = true;

        [JsonProperty("sectionType")]
        public ESectionType SectionType { get; set; } = 0;

        public FileSectionNode ToModel()
        {
            return new FileSectionNode()
            {
                id = Guid.NewGuid(),
                title = Title,
                subtitle = Subtitle,
                isExpanded = IsExpanded,
                isVisible = IsVisible,
                sectionType = SectionType
            };
        }
    }
}
