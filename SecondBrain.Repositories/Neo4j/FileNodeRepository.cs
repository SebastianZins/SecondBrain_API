using Neo4jClient;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.DatabaseModels.Neo4j;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
