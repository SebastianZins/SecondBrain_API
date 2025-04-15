using Newtonsoft.Json;
using SecondBrain.Models.DatabaseModels.MongoDB.Section;

namespace SecondBrain.Models.DTOs.FileSection.ChecklistSection
{
    public class ChecklistItemDTO
    {
        [JsonProperty("id")]
        public Guid? Id { get; set; } = null;

        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;

        [JsonProperty("isChecked")]
        public bool IsChecked { get; set; } = false;

        [JsonProperty("checkedDate")]
        public long? CheckedDate { get; set; } = null;
        public ChecklistItemDTO() { }

        public ChecklistItemDTO(ChecklistItemModel node)
        {
            Text = node.text;
            IsChecked = node.isChecked;
            CheckedDate = node.checkedDate;
        }

        public ChecklistItemModel ToModel()
        {
            return new ChecklistItemModel()
            {
                id = Id != null ? (Guid)(Id) : Guid.NewGuid(),
                text = Text,
                isChecked = IsChecked,
                checkedDate = CheckedDate,
            };
        }
    }
}
