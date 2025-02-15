using Newtonsoft.Json;
using SecondBrain.Core.Enums;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Models.DTOs.FileStructure
{
    public class FileStructureGetResponseDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("treeId")]
        public int TreeId { get; set; } = 0;

        [JsonProperty("label")]
        public string Label { get; set; } = string.Empty;

        [JsonProperty("type")]
        public EFileType Type { get; set; } = 0;

        [JsonProperty("children")]
        public List<FileStructureGetResponseDTO>? Children { get; set; } = null;

        public FileStructureGetResponseDTO(FileStructureNode folder)
        {
            Id = folder.id;
            Label = folder.label;
            Type = folder.type;
            TreeId = folder.treeId;
            if (folder.type == EFileType.FOLDER)
            {
                Children = new List<FileStructureGetResponseDTO>();
            }
        }
    }
}
