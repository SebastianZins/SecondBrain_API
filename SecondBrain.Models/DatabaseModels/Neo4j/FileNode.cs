namespace SecondBrain.Models.DatabaseModels.Neo4j
{
    public class FileNode : BaseNode
    {
        public string name { get; set; } = string.Empty;
        public List<Guid> tags { get; set; } = new List<Guid>();
        public int category { get; set; } = 0;
    }
}
