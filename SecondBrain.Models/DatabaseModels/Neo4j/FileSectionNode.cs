using SecondBrain.Core.Enums;

namespace SecondBrain.Models.DatabaseModels.Neo4j
{
    public class FileSectionNode : BaseNode
    {
        public string title { get; set; } = string.Empty;
        public string subtitle { get; set; } = string.Empty;
        public bool isExpanded { get; set; } = true;
        public bool isVisible { get; set; } = true;
        public int orderId { get; set; } = 0;
        public ESectionType sectionType { get; set; } = 0;
    }
}
