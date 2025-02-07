namespace SecondBrain.Models.DatabaseModels.Neo4j
{
    public class UserNode : BaseNode
    {
        public string firstName {  get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string passwordSalt { get; set; } = string.Empty;
        public long created { get; set; } = 0;
        public string? refreshToken {  get; set; }
    }
}
