using Newtonsoft.Json;

namespace SecondBrain.Models.DTOs.Auth
{
    public class LoginRequestDTO
    {
        [JsonProperty("email")]
        public string Email {  get; set; } = string.Empty;

        [JsonProperty("password")]
        public string Password { get; set; } = string.Empty;
    }
}
