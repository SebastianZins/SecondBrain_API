using Newtonsoft.Json;

namespace SecondBrain.Models.DTOs.FileNode
{
    public class FileNodeResponseDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        public FileNodeResponseDTO FromModel(Models.DatabaseModels.Neo4j.FileNode node)
        {
            Name = node.name;
            return this;
        }
    }
}
