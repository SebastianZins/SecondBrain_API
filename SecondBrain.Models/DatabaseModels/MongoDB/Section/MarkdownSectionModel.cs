namespace SecondBrain.Models.DatabaseModels.MongoDB.Section
{
    public class MarkdownSectionModel : MongoDBBaseModel
    {
        public string text { get; set; } = string.Empty;
    }
}
