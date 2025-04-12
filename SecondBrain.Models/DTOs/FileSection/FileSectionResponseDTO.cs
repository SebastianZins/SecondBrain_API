using Newtonsoft.Json;
using SecondBrain.Core.Enums;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Models.DTOs.FileSection
{
    public class FileSectionResponseDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

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

        [JsonProperty("tags")]
        public List<string> Tags { get; set; } = new List<string>();

        public FileSectionResponseDTO() { }

        public FileSectionResponseDTO(FileSectionNode node)
        {
            Id = node.id;
            Title = node.title;
            Subtitle = node.subtitle;
            SectionType = node.sectionType;
            IsExpanded = node.isExpanded;
            IsVisible = node.isVisible;
        }
    }
}
