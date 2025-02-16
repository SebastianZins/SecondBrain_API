using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using SecondBrain.Models.DatabaseModels.MongoDB.File;
using SecondBrain.Models.DatabaseModels.MongoDB.Section;

namespace SecondBrain.Database.MongoDB
{
    public class FileSectionContext
    {
        private readonly IMongoDatabase _database;

        public FileSectionContext(IOptions<MongoDbSettingsModel> settings)
        {
            var mongoDbSettings = settings.Value;
            var client = new MongoClient(mongoDbSettings.ConnectionString);
            _database = client.GetDatabase(mongoDbSettings.SectionDatabaseName);
        }

        public IMongoCollection<ChecklistSectionModel> Checklist
        {
            get
            {
                return _database.GetCollection<ChecklistSectionModel>("checklist_section");
            }
        }

        public IMongoCollection<ListSectionModel> List
        {
            get
            {
                return _database.GetCollection<ListSectionModel>("list_section");
            }
        }

        public IMongoCollection<MarkdownSectionModel> Markdown
        {
            get
            {
                return _database.GetCollection<MarkdownSectionModel>("markdown_section");
            }
        }

        public IMongoCollection<OverviewSectionModel> Overview
        {
            get
            {
                return _database.GetCollection<OverviewSectionModel>("overview_section");
            }
        }

        public IMongoCollection<TableSectionModel> Table
        {
            get
            {
                return _database.GetCollection<TableSectionModel>("table_section");
            }
        }

        public IMongoCollection<TextSectionModel> Text
        {
            get
            {
                return _database.GetCollection<TextSectionModel>("text_section");
            }
        }
    }
}
