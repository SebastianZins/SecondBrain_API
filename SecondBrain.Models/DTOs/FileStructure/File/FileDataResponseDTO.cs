using Newtonsoft.Json;
using SecondBrain.Models.DatabaseModels.Neo4j;
using SecondBrain.Models.DTOs.FileSection.ChecklistSection;
using SecondBrain.Models.DTOs.FileSection.ListSelection;
using SecondBrain.Models.DTOs.FileSection.MarkdownSection;
using SecondBrain.Models.DTOs.FileSection.OverviewSection;
using SecondBrain.Models.DTOs.FileSection.TableSection;
using SecondBrain.Models.DTOs.FileSection.TextSection;

namespace SecondBrain.Models.DTOs.FileStructure.File
{
    public class FileDataResponseDTO : FileResponseDTO
    {
        [JsonProperty("textSections")]
        public List<TextSectionResponseDTO> TextSections { get; set; } = new List<TextSectionResponseDTO>();

        [JsonProperty("markdownSections")]
        public List<MarkdownSectionResponseDTO> MarkdownSections { get; set; } = new List<MarkdownSectionResponseDTO>();

        [JsonProperty("listSections")]
        public IEnumerable<ListSectionResponseDTO> ListSections { get; set; } = new List<ListSectionResponseDTO>();

        [JsonProperty("checkListSections")]
        public List<ChecklistSectionResponseDTO> CheckListSections { get; set; } = new List<ChecklistSectionResponseDTO>();

        [JsonProperty("tableSections")]
        public List<TableSectionResponseDTO> TableSections { get; set; } = new List<TableSectionResponseDTO>();

        [JsonProperty("overviewSections")]
        public List<OverviewSectionResponseDTO> OverviewSections { get; set; } = new List<OverviewSectionResponseDTO>();

        public FileDataResponseDTO(FileNode node) : base(node) {}
    }
}
