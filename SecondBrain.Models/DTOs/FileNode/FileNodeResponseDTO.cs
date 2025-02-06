using Newtonsoft.Json;

namespace SecondBrain.Models.DTOs.FileNode
{
    public class FileNodeResponseDTO
    {
        [JsonProperty("id")]
        public Guid? Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("created")]
        public long? Created { get; set; }

        [JsonProperty("createdBy")]
        public Guid? CreatedBy { get; set; }

        [JsonProperty("updated")]
        public long? Updated { get; set; }

        [JsonProperty("updatedBy")]
        public Guid? UpdatedBy { get; set; }

        [JsonProperty("tags")]
        public List<Guid> Tags { get; set; } = new List<Guid>();

        [JsonProperty("category")]
        public int Category { get; set; } = 0;

        public FileNodeResponseDTO() {}

        public FileNodeResponseDTO(Models.DatabaseModels.Neo4j.FileNode node)
        {
            Id = node.id;
            Name = node.name;
            Created = node.created;
            CreatedBy = node.createdBy;
            Updated = node.updated;
            UpdatedBy = node.updatedBy;
            Tags = node.tags;
            Category = node.category;
        }

        public Models.DatabaseModels.Neo4j.FileNode ToModel()
        {
            return new Models.DatabaseModels.Neo4j.FileNode()
            {
                id = Id != null ? (Guid)Id : Guid.NewGuid(),
                name = Name,
                created = Created != null ? (long)Created : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                createdBy = CreatedBy,
                updated = Updated != null ? (long)Updated : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                updatedBy = UpdatedBy,
                tags = Tags,
                category = Category
            };
        }
    }
}
