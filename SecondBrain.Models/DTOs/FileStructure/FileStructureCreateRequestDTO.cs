using Newtonsoft.Json;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Models.DTOs.FileStructure
{
    public class FileStructureCreateRequestDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("icon")]
        public string? Icon { get; set; } = null;

        [JsonProperty("parentFolder")]
        public Guid? ParentFolder { get; set; } = null;

        [JsonProperty("fileClass")]
        public Guid? FileClass { get; set; } = null;

        [JsonProperty("showCreateTemplateBtn")]
        public bool ShowCreateTemplateBtn { get; set; } = false;

        public FileStructureNode ToModel()
        {
            return new FileStructureNode()
            {
                id = Guid.NewGuid(),
                name = Name,
                icon = Icon,
                fileClass = FileClass,
                showCreateTemplateBtn = ShowCreateTemplateBtn
            };
        }
    }
}
