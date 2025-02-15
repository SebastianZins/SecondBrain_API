using Newtonsoft.Json;
using SecondBrain.Core.Enums;

namespace SecondBrain.Models.DTOs.FileStructure
{
    public class FileStructureUpdateRequestDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("label")]
        public string Label { get; set; } = string.Empty;

        [JsonProperty("type")]
        public EFileType Type { get; set; } = 0;
    }
}
