using Newtonsoft.Json;

namespace SecondBrain.Models.DTOs.FileStructure
{
    public class FileStructureMoveRequestDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("parentId")]
        public Guid? ParentId { get; set; }

        [JsonProperty("oldTreeId")]
        public int OldTreeId { get; set; } = 0;

        [JsonProperty("newTreeId")]
        public int NewTreeId { get; set; } = 0;
    }
}
