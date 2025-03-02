using Newtonsoft.Json;
using SecondBrain.Models.DatabaseModels.MongoDB.Section;

namespace SecondBrain.Models.DTOs.FileSection.ChecklistSection
{
    public class ChecklistItemResponseDTO
    {
        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;

        [JsonProperty("isChecked")]
        public bool IsChecked { get; set; } = false;

        [JsonProperty("checkedDate")]
        public long CheckedDate { get; set; } = 0;

        public ChecklistItemResponseDTO(ChecklistItemModel node)
        {
            Text = node.text;
            IsChecked = node.isChecked;
            CheckedDate = node.checkedDate;
        }
    }
}
