using Newtonsoft.Json;

namespace SecondBrain.Models.DTOs.FileNode
{
    public class FileNodeCreateRequestDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("tags")]
        public List<Guid> Tags { get; set; } = new List<Guid>();

        [JsonProperty("category")]
        public int Category { get; set; } = 0;

        public Models.DatabaseModels.Neo4j.FileNode ToModel()
        {
            return new Models.DatabaseModels.Neo4j.FileNode()
            {
                id = Guid.NewGuid(),
                name = Name,
                tags = Tags,
                category = Category
            };
        }
    }
}