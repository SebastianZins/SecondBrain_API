using Newtonsoft.Json;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Models.DTOs.FileStructure.File
{
    public class FileUpdateRequestDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

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

        public FileNode WriteToModel(FileNode node)
        {
            node.id = Id;
            node.title = Title;
            node.subtitle = Subtitle;
            node.tags = Tags;
            node.category = Category;
            node.fileClasses = FileClasses;
            return node;
        }
    }
}
