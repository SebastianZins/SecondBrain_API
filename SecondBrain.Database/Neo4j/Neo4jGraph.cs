using Microsoft.Extensions.Options;
using Neo4jClient;

namespace SecondBrain.Database.Neo4j
{
    public class Neo4jGraph
    {
        private readonly IGraphClient _client;

        public Neo4jGraph(IOptions<Neo4jSettingsModel> settings)
        {
            var neo4jSettings = settings.Value;
            _client = new BoltGraphClient(new Uri(neo4jSettings.Uri), neo4jSettings.Username, neo4jSettings.Password);
            _client.ConnectAsync().Wait();
        }

        public IGraphClient GetClient()
        {
            return _client;
        }
    }
}
