using Neo4jClient;
using SecondBrain.Database.Neo4j;
using SecondBrain.Interfaces.Neo4j;

namespace SecondBrain.Repositories.Neo4j
{
    public class Neo4jRepository : INeo4jRepository
    {
        protected readonly IGraphClient _graph;

        public Neo4jRepository(Neo4jGraph graph)
        {
            _graph = graph.GetClient();
        }
    }
}
