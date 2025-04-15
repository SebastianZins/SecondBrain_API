using Neo4j.Driver;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Repositories.Neo4j
{
    public class UserRepository : Neo4jRepository
    {
        public UserRepository(Neo4jGraph graph) : base(graph) { }

        /// <summary>
        /// Get User by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<UserNode> GetByIdAsync(Guid id)
        {
            var query = _graph.Cypher
                          .Match("(user:User)")
                          .Where((UserNode user) => user.id == id)
                          .Return(user => user.As<UserNode>());

            return (await query.ResultsAsync).Single();
        }

        /// <summary>
        /// Get User by mail
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<UserNode> GetByMailAsync(string mail)
        {
            var query = _graph.Cypher
                       .Match("(user:User)")
                       .Where((UserNode user) => user.email == mail)
                       .Return(user => user.As<UserNode>());

            return (await query.ResultsAsync).Single();
        }

        /// <summary>
        /// Update User data 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task UpdateAsync(UserNode user)
        {
            var query = _graph.Cypher
                               .Match("(user:User{id: $id})")
                               .Set("user = $user")
                               .WithParams(new { id = user.id, user });

            await query.ExecuteWithoutResultsAsync();
        }

        /// <summary>
        /// Create User data 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task CreateAsync(UserNode user)
        {
            var query = _graph.Cypher
                                .Merge("(user:User{id : $id})")
                                .OnCreate()
                                .Set("user = $user")
                                .WithParams(new { user, user.id });

            await query.ExecuteWithoutResultsAsync();
        }

        /// <summary>
        /// Delete User 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task DeleteAsync(Guid id)
        {
            var query = _graph.Cypher
                               .Match("(user:User)")
                               .Where((UserNode user) => user.id == id)
                               .DetachDelete("user");

            await query.ExecuteWithoutResultsAsync();
        }
    }
}
