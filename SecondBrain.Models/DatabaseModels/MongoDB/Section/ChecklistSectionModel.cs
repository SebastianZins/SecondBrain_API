using SecondBrain.Models.DatabaseModels.MongoDB.Section;

namespace SecondBrain.Models.DatabaseModels.MongoDB.File
{
    public class ChecklistSectionModel : MongoDBBaseModel
    {
        public List<ChecklistItemModel> items { get; set; } = new List<ChecklistItemModel>();
    }
}
