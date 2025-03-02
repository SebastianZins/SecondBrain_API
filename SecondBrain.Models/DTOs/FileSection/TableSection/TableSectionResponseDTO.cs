using Newtonsoft.Json;
using SecondBrain.Models.DatabaseModels.MongoDB.Section;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Models.DTOs.FileSection.TableSection
{
    public class TableSectionResponseDTO : FileSectionResponseDTO
    {
        [JsonProperty("numRows")]
        public int NumRows { get; set; } = 0;

        [JsonProperty("numColumns")]
        public int NumColumns { get; set; } = 0;

        [JsonProperty("columnNames")]
        public List<string> ColumnNames { get; set; } = new List<string>();

        [JsonProperty("data")]
        public List<List<string>> Data { get; set; } = new List<List<string>>();

        public TableSectionResponseDTO(FileSectionNode metaData, TableSectionModel data) : base(metaData)
        {
            NumRows = data.numRows;
            NumColumns = data.numColumns;
            ColumnNames = data.columnNames;
            Data = data.data;
        }
    }
}
