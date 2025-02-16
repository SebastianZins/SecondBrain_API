using Neo4j.Driver;
using Neo4jClient;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Repositories.Neo4j
{
    public class FileRepository
    {
        private readonly IGraphClient _graph;

        public FileRepository(Neo4jGraph graph)
        {
            _graph = graph.GetClient();
        }

        /// <summary>
        /// Get File info by id
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<FileNode> GetByIdAsync(Guid fileId, Guid userId)
        {
            try
            {
                var query = _graph.Cypher
                    .Match("(file:File)-[:CreatedBy|UpdatedBy]->(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .ReturnDistinct(file => file.As<FileNode>());

                return (await query.ResultsAsync).Single();
            }
            catch (Exception e)
            {
                Console.Write("Error: ", e.Message);
                throw new Neo4jException("Error: File not found");
            }
        }

        /// <summary>
        /// Get File info by file structure item (so by path)
        /// </summary>
        /// <param name="fileStructureId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<FileNode> GetByFileStructureItemAsync(Guid fileStructureId, Guid userId)
        {
            try
            {
                var query = _graph.Cypher
                    .Match("(user: User)")
                    .Where((UserNode user) => user.id == userId)
                    .Match("(user)<-[:CreatedBy|UpdatedBy]-(item:FileStructure)-[:StructureItemOf]->(file:File)")
                    .Where((FileStructureNode item) => item.id == fileStructureId)
                    .ReturnDistinct(file => file.As<FileNode>());

                return (await query.ResultsAsync).Single();
            }
            catch (Exception e)
            {
                Console.Write("Error: ", e.Message);
                throw new Neo4jException("Error: File not found");
            }
        }

        /// <summary>
        /// Create File
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fileStructureId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<FileNode> CreateAsync(FileNode data, Guid fileStructureId, Guid userId)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            try
            {
                var query = _graph.Cypher
                    .Match("(user: User)")
                    .Where((UserNode user) => user.id == userId)
                    .Match("(item: FileStructure)")
                    .Where((FileStructureNode item) => item.id == fileStructureId)
                    .Merge("(file:File{{id:$data.id}})")
                    .OnCreate()
                    .Set("file = $data")
                    .Merge("(file)<-[:StructureItemOf]-(item)")
                    .Merge("(file)-[:CreatedBy {created:$now}]->(user)")
                    .Merge("(file)-[:UpdatedBy {updated:$now}]->(user)")
                    .WithParams(new {now, data})
                    .ReturnDistinct(file => file.As<FileNode>());

                return (await query.ResultsAsync).Single();
            }
            catch (Exception e)
            {
                Console.Write("Error: ", e.Message);
                throw new Neo4jException("Error: File could not be created");
            }
        }

        /// <summary>
        /// Update File data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<FileNode> UpdateAsync(FileNode data, Guid userId)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            try
            {
                var query = _graph.Cypher
                    .Match("(user:User)<-[]-(file:File)")
                    .Where((UserNode user) => user.id == userId)
                    .AndWhere((FileNode file) => file.id == data.id)
                    .Set("file.name = $data.name")
                    .Set("file.title = $data.title")
                    .Set("file.subtitle = $data.subtitle")
                    .Set("file.tags = $data.tags")
                    .Set("file.category = $data.category")
                    .Set("file.fileClasses = $data.fileClasses")
                    .With("file, user")
                    .Match("(file)-[rel:UpdatedBy]->(:User)")
                    .Delete("rel")
                    .With("file, user")
                    .Merge("(file)-[:UpdatedBy {updated:$now}]->(user)")
                    .WithParams(new { now, data })
                    .ReturnDistinct(file => file.As<FileNode>());

                return (await query.ResultsAsync).Single();
            }
            catch (Exception e)
            {
                Console.Write("Error: ", e.Message);
                throw new Neo4jException("Error: File data could not be updated");
            }
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task DeleteAsync(Guid fileId, Guid userId)
        {
            try
            {
                var query = _graph.Cypher
                    .Match("(user: User)<-[:CreatedBy|UpdatedBy]-(file:File)")
                    .Where((UserNode user) => user.id == userId)
                    .AndWhere((FileNode file) => file.id == fileId)
                    .Match("(file)-[:Contains]->(section:FileSection)-[]-(attachment:Attachment)")
                    .DetachDelete("attachment")
                    .DetachDelete("section")
                    .DetachDelete("file");

                await query.ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {
                Console.Write("Error: ", e.Message);
                throw new Neo4jException("Error: File could not be deleted");
            }
        }
    }
}
