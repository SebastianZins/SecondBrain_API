using Newtonsoft.Json;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Models.DTOs.FileStructure.File
{
    public class FileResponseDTO : FileStructureGetResponseDTO
    {
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("subtitle")]
        public string Subtitle { get; set; } = string.Empty;

        [JsonProperty("tags")]
        public List<Guid> Tags { get; set; } = new List<Guid>();

        [JsonProperty("category")]
        public int Category { get; set; } = 0;

        [JsonProperty("fileClasses")]
        public List<Guid> FileClasses { get; set; } = new List<Guid>();

        [JsonProperty("sectionsOrder")]
        public List<Guid> SectionsOrder { get; set; } = new List<Guid>();

        public FileResponseDTO(FileNode node) : base(node)
        {
            Id = node.id;
            Title = node.title;
            Subtitle = node.subtitle;
            Tags = node.tags;
            Category = node.category;
            FileClasses = node.fileClasses;
            SectionsOrder = node.sectionsOrder;
        }
    }
}
