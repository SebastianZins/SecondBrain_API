using MongoDB.Driver;
using Neo4j.Driver;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Repositories.Neo4j
{
    public class TagRepository : Neo4jRepository
    {
        public TagRepository(Neo4jGraph graph) : base(graph) { }

        /// <summary>
        /// Get tags of file section
        /// </summary>
        /// <param name="FileSectionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<List<string>> GetTagsOfFileSectionAsync(Guid FileSectionId, Guid userId)
        {
            var query = _graph.Cypher
                    .Match("(tag:Tag)-[:MentionedIn]->(section:FileSection)-[:CreatedBy|UpdatedBy]->(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .AndWhere((FileSectionNode section) => section.id == FileSectionId)
                    .ReturnDistinct(tag => tag.As<TagNode>().name);

            return (await query.ResultsAsync).ToList();
        }

        public async Task UpdateFileSectionTagsAsync(List<string> tags, Guid FileSectionId, Guid userId)
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

        public async Task RemoveFileSectionTagsAsync(List<string> tags, Guid FileSectionId, Guid userId)
        {
            var query = _graph.Cypher
                    .Match("(tag:Tag)-[rel:MentionedIn]->(section:FileSection)-[:CreatedBy|UpdatedBy]->(user:User)")
                    .Where((UserNode user) => user.id == userId)
                    .AndWhere((FileSectionNode section) => section.id == FileSectionId)
                    .AndWhere($"tag.name IN [{string.Join(',', tags.Select(t => $"'{t}'"))}]")
                    .Delete("rel");
            await query.ExecuteWithoutResultsAsync();
        }

        public async Task<List<string>> GetTagsAsync(List<string> tags)
        {
            var query = _graph.Cypher
                     .Match("(tag:Tag)")
                     .Where($"tag.name IN [{string.Join(',', tags.Select(t => $"'{t}'"))}]")
                     .ReturnDistinct(tag => tag.As<TagNode>().name);

            return (await query.ResultsAsync).ToList();
        }

        public async Task CreateTagsAsync(string tag)
        {
            var query = _graph.Cypher
                .Merge("(tag:Tag{name:tag})");
            await query.ExecuteWithoutResultsAsync();
        }

        public async Task DeleteOrphanTagsAsync(List<string> tags)
        {
            var query = _graph.Cypher
                    .Match("(tag:Tag)")
                    .Where($"tag.name IN [{string.Join(',', tags.Select(t => $"'{t}'"))}]")
                    .AndWhere("NOT(tag)-[]-()")
                    .Delete("tag");
            await query.ExecuteWithoutResultsAsync();
        }
    }
}
