using MongoDB.Driver;
using Neo4j.Driver;
using Neo4jClient;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.DatabaseModels.Neo4j;
using static System.Collections.Specialized.BitVector32;

namespace SecondBrain.Repositories.Neo4j
{
    public class TagRepository
    {
        private readonly IGraphClient _graph;

        public TagRepository(Neo4jGraph graph)
        {
            _graph = graph.GetClient();
        }

        /// <summary>
        /// Get tags of file section
        /// </summary>
        /// <param name="FileSectionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<List<string>> GetTagsOfFileSectionAsync(Guid FileSectionId, Guid userId)
        {
            try
            {
                var query = _graph.Cypher
                    .Match("(tag:Tag)-[:MentionedIn]->(section:FileSection)-[:CreatedBy|UpdatedBy]->(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .AndWhere((FileSectionNode section) => section.id == FileSectionId)
                    .ReturnDistinct(tag => tag.As<TagNode>().name);

                return (await query.ResultsAsync).ToList();
            }
            catch (Exception e)
            {
                Console.Write("Error: ", e.Message);
                throw new Neo4jException("Error: Tags of file section could not be loaded");
            };
        }

        public async Task UpdateFileSectionTagsAsync(List<string> tags, Guid FileSectionId, Guid userId)
        {
            try
            {
                foreach (var tag in tags)
                {
                    var query = _graph.Cypher
                        .Merge($"(tag:Tag {{name: '{tag}'}})")
                        .With("tag")
                        .Match("(section:FileSection)-[:CreatedBy|UpdatedBy]->(user:User)")
                        .Where((UserNode user) => user.id == userId)
                        .AndWhere((FileSectionNode section) => section.id == FileSectionId)
                        .Merge("(tag)-[:MentionedIn]->(section)");
                    await query.ExecuteWithoutResultsAsync();
                }
            }
            catch (Exception e)
            {
                Console.Write("Error: ", e.Message);
                throw new Neo4jException("Error: Tags could not be updated");
            };
        }

        public async Task RemoveFileSectionTagsAsync(List<string> tags, Guid FileSectionId, Guid userId)
        {
            try
            {
                var query = _graph.Cypher
                    .Match("(tag:Tag)-[rel:MentionedIn]->(section:FileSection)-[:CreatedBy|UpdatedBy]->(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .AndWhere((FileSectionNode section) => section.id == FileSectionId)
                    .AndWhere($"tag.name IN [{string.Join(',', tags.Select(t => $"'{t}'"))}]")
                    .Delete("rel");
                await query.ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {
                Console.Write("Error: ", e.Message);
                throw new Neo4jException("Error: Tags could not be updated");
            };
        }

        public async Task<List<string>> GetTagsAsync(List<string> tags)
        {
            try
            {
                var query = _graph.Cypher
                    .Match("(tag:Tag)")
                    .Where($"tag.name IN [{string.Join(',', tags.Select(t => $"'{t}'"))}]")
                    .ReturnDistinct(tag => tag.As<TagNode>().name);

                return (await query.ResultsAsync).ToList();
            }
            catch (Exception e)
            {
                Console.Write("Error: ", e.Message);
                throw new Neo4jException("Error: Tags could not be loaded");
            };
        }

        public async Task CreateTagsAsync(string tag)
        {
            try
            {
                var query = _graph.Cypher
                    .Merge("(tag:Tag{name:tag})");
                await query.ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {
                Console.Write("Error: ", e.Message);
                throw new Neo4jException("Error: Tag could not be created");
            };
        }

        public async Task DeleteOrphanTagsAsync(List<string> tags)
        {
            try
            {
                var query = _graph.Cypher
                    .Match("(tag:Tag)")
                    .Where($"tag.name IN [{string.Join(',', tags.Select(t => $"'{t}'"))}]")
                    .AndWhere("NOT(tag)-[]-()")
                    .Delete("tag");
                await query.ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {
                Console.Write("Error: ", e.Message);
                throw new Neo4jException("Error: Tags could not be deleted");
            };
        }
    }
}
