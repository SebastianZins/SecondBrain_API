using SecondBrain.Core.Enums;

namespace SecondBrain.Models.DatabaseModels.Neo4j
{
    public class FileStructureNode : BaseNode
    {
        public int treeId { get; set; } = 0;
        public string label { get; set; } = string.Empty;
        public EFileType type { get; set; } = EFileType.FOLDER;
    }
}
