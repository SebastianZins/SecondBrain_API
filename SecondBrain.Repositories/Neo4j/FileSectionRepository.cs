using Neo4j.Driver;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Repositories.Neo4j
{
    public class FileSectionRepository : Neo4jRepository
    {
        public FileSectionRepository(Neo4jGraph graph) : base(graph) { }

        public async Task<List<FileSectionNode>> GetAllSectionsByFolder(Guid folderId, Guid userId)
        {
            var query = _graph.Cypher
                .Match("(user:User)<-[:CreatedBy|UpdatedBy]-(folder:FileStructure)")
                .Where((UserNode user) => user.id == userId)
                .AndWhere((FileStructureNode folder) => folder.id == folderId)
                .Match("(folder)-[:ParentFolderOf*]->(:FileStructure)-[:StructureItemOf]->(:File)-[:Contains]->(section:FileSection)-[:CreatedBy|UpdatedBy]->(user)")
                .ReturnDistinct(section => section.As<FileSectionNode>());

            return (await query.ResultsAsync).ToList();
        }

        /// <summary>
        /// Get Section info by id
        /// </summary>
        /// <param name="sectionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<FileSectionNode> GetByIdAsync(Guid sectionId, Guid userId)
        {
            var query = _graph.Cypher
                .Match("(section:FileSection)-[:CreatedBy|UpdatedBy]->(user:User)")
                .Where((UserNode user) => user.id == userId)
                .AndWhere((FileSectionNode section) => section.id == sectionId)
                .ReturnDistinct(section => section.As<FileSectionNode>());

            return (await query.ResultsAsync).Single();
        }

        /// <summary>
        /// Get sections by file
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<List<FileSectionNode>> GetByFileIdAsync(Guid fileId, Guid userId)
        {
            var query = _graph.Cypher
                .Match("(user: User)")
                .Where((UserNode user) => user.id == userId)
                .Match("(user)<-[:CreatedBy|UpdatedBy]-(file:File)-[:Contains]->(section:FileSection)")
                .Where((FileNode file) => file.id == fileId)
                .ReturnDistinct(section => section.As<FileSectionNode>());

            return (await query.ResultsAsync).ToList();
        }

        /// <summary>
        /// Get sections by file
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<List<Guid>> GetIdsByFileIdAsync(Guid fileId, Guid userId)
        {
            var query = _graph.Cypher
                .Match("(user: User)")
                .Where((UserNode user) => user.id == userId)
                .Match("(user)<-[:CreatedBy|UpdatedBy]-(file:File)-[:Contains]->(section:FileSection)")
                .Where((FileNode file) => file.id == fileId)
                .ReturnDistinct(section => section.As<FileSectionNode>().id);

            return (await query.ResultsAsync).ToList();
        }

        /// <summary>
        /// Get sections by file
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<List<Guid>> GetByFolderIdAsync(Guid folderId, Guid userId)
        {
            var query = _graph.Cypher
                .Match("(user: User)<-[:CreatedBy|UpdatedBy]-(folder:Folder)")
                .Where((UserNode user) => user.id == userId)
                .AndWhere((FileStructureNode folder) => folder.id == folderId)
                .OptionalMatch("(folder)-[:ParentFolderOf*]->(:Folder)-[:Contains]->(file:File)-[:Contains]->(section:FileSection)")
                .OptionalMatch("(folder)-[:Contains]->(file:File)-[:Contains]->(section:FileSection)")
                .ReturnDistinct(section => section.As<FileSectionNode>().id);

            return (await query.ResultsAsync).ToList();
        }

        /// <summary>
        /// Create File section
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fileStructureId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<FileSectionNode> CreateAsync(FileSectionNode data, Guid fileId, Guid userId)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var query = _graph.Cypher
                .Match("(user: User)")
                .Where((UserNode user) => user.id == userId)
                .Match("(file: File)")
                .Where((FileNode file) => file.id == fileId)
                .Merge($"(section:FileSection{{id:$data.id}})")
                .OnCreate()
                .Set("section = $data")
                .Merge("(section)<-[:Contains]-(file)")
                .Merge("(section)-[:CreatedBy {created:$now}]->(user)")
                .Merge("(section)-[:UpdatedBy {updated:$now}]->(user)")
                .WithParams(new { now, data })
                .ReturnDistinct(section => section.As<FileSectionNode>());

            return (await query.ResultsAsync).Single();
        }

        /// <summary>
        /// Update File section data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task UpdateAsync(FileSectionNode data, Guid userId)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var query = _graph.Cypher
                .Match("(user:User)<-[]-(section:FileSection)")
                .Where((UserNode user) => user.id == userId)
                .AndWhere((FileSectionNode section) => section.id == data.id)
                .Set("section.title = $data.title")
                .Set("section.subtitle = $data.subtitle")
                .Set("section.isExpanded = $data.isExpanded")
                .Set("section.isVisible = $data.isVisible")
                .Set("section.sectionType = $data.sectionType")
                .With("section, user")
                .Match("(section)-[rel:UpdatedBy]->(:User)")
                .Delete("rel")
                .With("section, user")
                .Merge("(section)-[:UpdatedBy {updated:$now}]->(user)")
                .WithParams(new { now, data });

            await query.ExecuteWithoutResultsAsync();
        }

        /// <summary>
        /// Delete file section
        /// </summary>
        /// <param name="sectionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task DeleteAsync(Guid sectionId, Guid userId)
        {
            var query = _graph.Cypher
                .Match("(user: User)<-[:CreatedBy|UpdatedBy]-(section:FileSection)")
                .Where((UserNode user) => user.id == userId)
                .AndWhere((FileSectionNode section) => section.id == sectionId)
                .OptionalMatch("(section)-[]-(attachment:Attachment)")
                .DetachDelete("attachment")
                .DetachDelete("section");

            await query.ExecuteWithoutResultsAsync();
        }
    }
}
