using Newtonsoft.Json;
using SecondBrain.Core.Enums;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Models.DTOs.FileStructure
{
    public class FileStructureGetResponseDTO
    {
        [JsonProperty("label")]
        public string Label { get; set; } = string.Empty;

        [JsonProperty("type")]
        public EFileType Type { get; set; } = 0;

        [JsonProperty("icon")]
        public string? Icon { get; set; }

        [JsonProperty("fileClass")]
        public Guid? FileClass { get; set; } = null;

        [JsonProperty("showBtn")]
        public bool ShowBtn { get; set; } = false;

        [JsonProperty("children")]
        public List<FileStructureGetResponseDTO>? Children { get; set; } = null;

        public FileStructureGetResponseDTO(FileStructureNode folder, EFileType type)
        {
            Label = folder.name;
            Type = type;
            Icon = folder.icon;
            FileClass = folder.fileClass;
            ShowBtn = folder.showCreateTemplateBtn;
            if (type == EFileType.FOLDER)
            {
                Children = new List<FileStructureGetResponseDTO>();
            }
        }
    }
}
