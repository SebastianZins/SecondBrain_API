using Newtonsoft.Json;
using SecondBrain.Core.Enums;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Models.DTOs.FileStructure.File
{
    public class FileCreateRequestDTO : FileStructureCreateRequestDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

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

        public FileNode ToModel()
        {
            return new FileNode()
            {
                id = Guid.NewGuid(),
                name = Name,
                title = Title,
                subtitle = Subtitle,
                tags = Tags,
                category = Category,
                fileClasses = FileClasses,
                label = Label,
                type = Type
            };
        }
    }
}