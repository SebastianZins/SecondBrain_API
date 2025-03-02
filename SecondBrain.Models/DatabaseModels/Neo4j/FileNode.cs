namespace SecondBrain.Models.DatabaseModels.Neo4j
{
    public class FileNode : FileStructureNode
    {
        public string title { get; set; } = string.Empty;
        public string subtitle { get; set; } = string.Empty;
        public List<Guid> tags { get; set; } = new List<Guid>();
        public int category { get; set; } = 0;
        public List<Guid> fileClasses { get; set; } = new List<Guid>();
        public List<Guid> sectionsOrder { get; set; } = new List<Guid>();
    }
}
