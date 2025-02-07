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

        public async Task<List<FileNode>> GetAllAsync()
        {
            try
            {
                var query = _graph.Cypher
                        .Match("(file:File)")
                        .Return(file => file.As<FileNode>());

                return (await query.ResultsAsync).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FileNode> GetByIdAsync(Guid id)
        {
            List<FileNode> result;
            try
            {
                var query = _graph.Cypher
                .Match("(file:File)")
                        .Where((FileNode file) => file.id == id)
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
            return result.Single();
        }

        public async Task<List<FileNode>> GetByNameAsync(string name)
        {
            try
            {
                var query = _graph.Cypher
                        .Match("(file:File)")
                        .Where($"toLower(file.name) CONTAINS {name.ToLower()}")
                        .Return(file => file.As<FileNode>());

                return (await query.ResultsAsync).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new Neo4jException("Error", "Getting files by name failed");
            }
        }

        public async Task<List<FileNode>> GetByPathAsync(string path)
        {
            throw new NotImplementedException();
        }

        public async Task UpsertAsync(FileNode file)
        {
            try
            {
                var query = _graph.Cypher
                        .Merge("(file:File{id: $id})")
                        .OnCreate()
                        .Set("file = $file")
                        .OnMatch()
                        .Set("file = $file")
                        .WithParams(new { id = file.id, file});

                await query.ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new Neo4jException("Error", "Upserting file data failed");
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var query = _graph.Cypher
                        .Match("(file:File)")
                        .Where((FileNode file) => file.id == id)
                        .Delete("file");

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
