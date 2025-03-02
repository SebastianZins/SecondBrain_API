namespace SecondBrain.Models.DatabaseModels.MongoDB.Section
{
    public class TableSectionModel : MongoDBBaseModel
    {
        public int numRows { get; set; } = 0;
        public int numColumns { get; set; } = 0;
        public List<string> columnNames { get; set; } = new List<string>();
        public List<List<string>> data { get; set; } = new List<List<string>>();
    }
}
