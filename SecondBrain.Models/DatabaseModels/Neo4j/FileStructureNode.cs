namespace SecondBrain.Models.DatabaseModels.Neo4j
{
    public class FileStructureNode : BaseNode
    {
        public string name { get; set; } = string.Empty;
        public string? icon { get; set; } = null;
        public Guid? fileClass { get; set; } = null;
        public bool showCreateTemplateBtn { get; set; } = false;
    }
}
