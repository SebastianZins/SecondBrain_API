namespace SecondBrain.Database.MongoDB
{
    public class MongoDbSettingsModel
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string FileDatabaseName { get; set; } = string.Empty;
        public string SectionDatabaseName { get; set; } = string.Empty;
    }
}
