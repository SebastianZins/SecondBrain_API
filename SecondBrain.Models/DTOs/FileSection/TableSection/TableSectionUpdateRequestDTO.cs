using Newtonsoft.Json;

namespace SecondBrain.Models.DTOs.FileSection.TableSection
{
    public class TableSectionUpdateRequestDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;
    }
}
