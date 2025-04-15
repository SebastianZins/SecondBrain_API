using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SecondBrain.Models.DatabaseModels.MongoDB.Section
{
    public class ChecklistItemModel
    {
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid id { get; set; } = Guid.NewGuid();
        public string text {  get; set; } = string.Empty;
        public bool isChecked { get; set; } = false;
        public long? checkedDate { get; set; } = null;
    }
}
