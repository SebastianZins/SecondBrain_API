using Newtonsoft.Json;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Models.DTOs.User
{
    public class UserResponseDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonProperty("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("created")]
        public long Created { get; set; } = 0;

        public UserResponseDTO(UserNode node)
        {
            Id = node.id;
            FirstName = node.firstName;
            LastName = node.lastName;
            Email = node.email;
            Created = node.created;
        }
    }
}
