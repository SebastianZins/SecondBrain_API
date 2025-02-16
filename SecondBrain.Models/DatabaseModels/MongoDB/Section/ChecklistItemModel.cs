namespace SecondBrain.Models.DatabaseModels.MongoDB.Section
{
    public class ChecklistItemModel : MongoDBBaseModel
    {
        public string text {  get; set; } = string.Empty;
        public bool isChecked { get; set; } = false;
        public long checkedDate { get; set; } = 0;
    }
}
