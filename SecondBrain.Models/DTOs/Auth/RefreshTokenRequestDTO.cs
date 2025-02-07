namespace SecondBrain.Models.DTOs.Auth
{
    public class RefreshTokenRequestDTO
    {
        public string token {  get; set; } = string.Empty;
        public string refreshToken { get; set; } = string.Empty;
    }
}
