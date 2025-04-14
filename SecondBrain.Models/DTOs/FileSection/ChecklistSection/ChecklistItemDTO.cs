using Newtonsoft.Json;
using SecondBrain.Models.DatabaseModels.MongoDB.Section;

namespace SecondBrain.Models.DTOs.FileSection.ChecklistSection
{
    public class ChecklistItemDTO
    {
        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;

        [JsonProperty("isChecked")]
        public bool IsChecked { get; set; } = false;

        [JsonProperty("checkedDate")]
        public long? CheckedDate { get; set; } = null;

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
                text = Text,
                isChecked = IsChecked,
                checkedDate = CheckedDate == null ? DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() : (long)(CheckedDate),
            };
        }
    }
}
