using SecondBrain.Models.DatabaseModels.MongoDB.Section;
using SecondBrain.Models.DatabaseModels.Neo4j;

namespace SecondBrain.Models.DTOs.FileSection.OverviewSection
{
    public class OverviewSectionResponseDTO : FileSectionResponseDTO
    {

        public OverviewSectionResponseDTO(FileSectionNode metaData, OverviewSectionModel data) : base(metaData)
        {
        }
    }
}
