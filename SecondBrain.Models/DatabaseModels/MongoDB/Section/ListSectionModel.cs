namespace SecondBrain.Models.DatabaseModels.MongoDB.Section
{
    public class ListSectionModel : MongoDBBaseModel
    {        
        public List<string> items { get; set; } = new List<string>();
    }
}
