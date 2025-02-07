namespace SecondBrain.Models.DatabaseModels.Neo4j
{
    public class FileNode : BaseNode
    {
        public string name { get; set; } = string.Empty;
        public long created { get; set; } = 0;
        public Guid? createdBy { get; set; } = Guid.Empty;
        public long updated { get; set; } = 0;
        public Guid? updatedBy { get; set; } = Guid.Empty;
        public List<Guid> tags { get; set; } = new List<Guid>();
        public int category { get; set; } = 0;
    }
}
