using Neo4j.Driver;
using Neo4jClient;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Repositories.Neo4j
{
    public class FileNodeRepository
    {
        private readonly IGraphClient _graph;

        public FileNodeRepository(Neo4jGraph graph)
        {
            _graph = graph.GetClient();
        }

        /// <summary>
        /// Get all File nodes
        /// </summary>
        /// <returns></returns>
        public async Task<List<FileNode>> GetAllAsync(Guid userId)
        {
            try
            {
                var query = _graph.Cypher
                    .Match("(file:File)-[]-(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .Return(file => file.As<FileNode>());

                return (await query.ResultsAsync).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get File node by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<FileNode> GetByIdAsync(Guid id, Guid userId)
        {
            List<FileNode> result;
            try
            {
                var query = _graph.Cypher
                .Match("(file:File)-[]-(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .AndWhere((FileNode file) => file.id == id)
                    .Return(file => file.As<FileNode>());

                result = (await query.ResultsAsync).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new Neo4jException("Error", "Getting files by name failed");
            }

            if (result.Count == 0)
            {
                throw new Neo4jException("Error", "File not found");
            }
            return result.First();
        }

        /// <summary>
        /// Get File nodes by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<List<FileNode>> GetByNameAsync(string name, Guid userId)
        {
            try
            {
                var query = _graph.Cypher
                    .Match("(file:File-[]-(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .AndWhere($"toLower(file.name) CONTAINS {name.ToLower()}")
                    .Return(file => file.As<FileNode>());

                return (await query.ResultsAsync).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new Neo4jException("Error", "Getting files by name failed");
            }
        }

        /// <summary>
        /// Get File nodes by path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<FileNode>> GetByPathAsync(string path, Guid userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update File node data
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task UpdateAsync(FileNode file, Guid userId)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            try
            {
                var query = _graph.Cypher
                    .Match("(file:File)-[rel:UpdatedBy]->(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .AndWhere((FileNode file) => file.id == file.id)
                    .Delete("rel")
                    .With("file, user")
                    .Merge("(file)-[rel:UpdatedBy{updated: $now}]->(user)")
                    .With("file")
                    .Set("file = $file")
                    .WithParams(new { file.id, file, now });


                await query.ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new Neo4jException("Error", "Updating file data failed");
            }
        }

        /// <summary>
        /// Create File node
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task CreateAsync(FileNode file, Guid userId)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            try
            {
                var query = _graph.Cypher
                    .Match("(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .Merge("(file:File{id:$id})")
                    .OnCreate()
                    .Set("file = $file")
                    .With("user, file")
                    .Merge("(file)-[:CreatedBy {created:$now}]->(user)")
                    .Merge("(file)-[:UpdatedBy {updated:$now}]->(user)")
                    .WithParams(new { file.id, file, now});

                await query.ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new Neo4jException("Error", "Creating file data failed");
            }
        }

        /// <summary>
        /// Delete File node
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task DeleteAsync(Guid id, Guid userId)
        {
            try
            {
                var query = _graph.Cypher
                    .Match("(file:File-[]-(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .AndWhere((FileNode file) => file.id == id)
                    .DetachDelete("file");

                await query.ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new Neo4jException("Error", "Deleting file failed");
            }
        }
    }
}
