using Neo4j.Driver;
using Neo4jClient;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Repositories.Neo4j
{
    public class UserRepository
    {
        private readonly IGraphClient _graph;

        public UserRepository(Neo4jGraph graph)
        {
            _graph = graph.GetClient();
        }

        /// <summary>
        /// Get User by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<UserNode> GetByIdAsync(Guid id)
        {
            try
            {
                var query = _graph.Cypher
                    .Match("(user:User)")
                    .Where((UserNode user) => user.id == id)
                    .Return(user => user.As<UserNode>());

                return (await query.ResultsAsync).Single();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new Neo4jException("Error", "Loading user by id failed");
            }
        }

        /// <summary>
        /// Get User by mail
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task<UserNode> GetByMailAsync(string mail)
        {
            try
            {
                var query = _graph.Cypher
                    .Match("(user:User)")
                    .Where((UserNode user) => user.email == mail)
                    .Return(user => user.As<UserNode>());

                return (await query.ResultsAsync).Single();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new Neo4jException("Error", "Loading user by mail failed");
            }
        }

        /// <summary>
        /// Update User data 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task UpdateAsync(UserNode user)
        {
            try
            {
                var query = _graph.Cypher
                    .Match("(user:User{id: $id})")
                    .Set("user = $user")
                    .WithParams(new { id = user.id, user });

                await query.ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new Neo4jException("Error", "Update user data failed");
            }
        }

        /// <summary>
        /// Create User data 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task CreateAsync(UserNode user)
        {
            try
            {
                var query = _graph.Cypher
                    .Merge("(user:User{id : $id})")
                    .OnCreate()
                    .Set("user = $user")
                    .WithParams(new {user, user.id});

                await query.ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new Neo4jException("Error", "Create user data failed");
            }
        }

        /// <summary>
        /// Delete User 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Neo4jException"></exception>
        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var query = _graph.Cypher
                    .Match("(user:User)")
                    .Where((UserNode user) => user.id == id)
                    .DetachDelete("user");

                await query.ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:", e.Message);
                throw new Neo4jException("Error", "Deleting user failed");
            }
        }
    }
}
