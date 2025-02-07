using Newtonsoft.Json;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Models.DTOs.User
{
    public class UserCreateRequestDTO
    {

        [JsonProperty("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonProperty("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("password")]
        public string password { get; set; } = string.Empty;

        public UserNode ToModel() {
            return new UserNode()
            {
                id = Guid.NewGuid(),
                firstName = FirstName,
                lastName = LastName,
                email = Email,
                created = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
        }

    }
}
