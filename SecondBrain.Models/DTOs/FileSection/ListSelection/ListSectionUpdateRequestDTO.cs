using Newtonsoft.Json;

namespace SecondBrain.Models.DTOs.FileSection.ListSelection
{
    public class ListSectionUpdateRequestDTO
    {
        [JsonProperty("id")]
        public Guid Id = Guid.Empty;

        [JsonProperty("items")]
        public List<string> Items = new List<string>();
    }
}
