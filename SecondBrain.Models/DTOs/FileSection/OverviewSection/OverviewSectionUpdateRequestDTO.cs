using Newtonsoft.Json;

namespace SecondBrain.Models.DTOs.FileSection.OverviewSection
{
    public class OverviewSectionUpdateRequestDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("numRows")]
        public int NumRows { get; set; } = 0;

        [JsonProperty("numColumns")]
        public int NumColumns { get; set; } = 0;

        [JsonProperty("columnNames")]
        public List<string> ColumnNames { get; set; } = new List<string>();

        [JsonProperty("data")]
        public List<List<string>> Data { get; set; } = new List<List<string>>();
    }
}
