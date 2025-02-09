using Newtonsoft.Json;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Models.DTOs.FileStructure
{
    public class FileStructureUpdateRequestDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("icon")]
        public string? Icon { get; set; } = null;

        [JsonProperty("fileClass")]
        public Guid? FileClass { get; set; } = null;

        [JsonProperty("showCreateTemplateBtn")]
        public bool ShowCreateTemplateBtn { get; set; } = false;

        public FileStructureNode WriteToModel(FileStructureNode folder)
        {
            folder.id = Id;
            folder.name = Name;
            folder.icon = Icon;
            folder.fileClass = FileClass;
            folder.showCreateTemplateBtn = ShowCreateTemplateBtn;
            return folder;
        }
    }
}
