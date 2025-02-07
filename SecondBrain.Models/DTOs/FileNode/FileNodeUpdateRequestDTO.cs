using Newtonsoft.Json;

namespace SecondBrain.Models.DTOs.FileNode
{
    public class FileNodeUpdateRequestDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("tags")]
        public List<Guid> Tags { get; set; } = new List<Guid>();

        [JsonProperty("category")]
        public int Category { get; set; } = 0;

        public Models.DatabaseModels.Neo4j.FileNode WriteToModel(Models.DatabaseModels.Neo4j.FileNode node)
        {
            node.id = Id;
            node.name = Name;
            node.tags = Tags;
            node.category = Category;
            return node;
        }
    }
}
