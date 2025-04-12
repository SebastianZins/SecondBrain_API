using SecondBrain.Models.DatabaseModels.MongoDB.Section;
using SecondBrain.Models.DatabaseModels.Neo4j;
using SecondBrain.Models.DTOs.FileSection;
using SecondBrain.Models.DTOs.FileSection.ListSelection;
using SecondBrain.Repositories.MongoDB;
using SecondBrain.Repositories.Neo4j;
using SecondBrain.Services.FileStructure;
using SecondBrain.Utils;
using System.Security.Claims;

namespace SecondBrain.Services.Section
{
    public class ListSectionDataService : FileSectionDataService
    {
        private readonly ListSectionRepository _dataRepo;
        private readonly FileSectionRepository _metaDataRepo;
        private readonly FileService _fileService;


        public ListSectionDataService(ListSectionRepository dataRepo, FileSectionRepository metaDataRepo, FileService fileService, TagRepository tagRepository) : base (tagRepository)
        {
            _metaDataRepo = metaDataRepo;
            _dataRepo = dataRepo;
            _dataRepo.CreateIndexAsync().Wait();

            _fileService = fileService;
        }

        /// <summary>
        /// Get File section by id
        /// </summary>
        /// <param name="sectionId"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<ListSectionResponseDTO> GetSectionByIdAsync(Guid sectionId, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            FileSectionNode metaData = await _metaDataRepo.GetByIdAsync(sectionId, userId);
            ListSectionModel data = await _dataRepo.GetByStructureIdAsync(metaData.id);

            return new ListSectionResponseDTO(metaData, data);
        }

        /// <summary>
        /// Get File section by file id
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<List<ListSectionResponseDTO>> GetBySectionFileIdAsync(Guid fileId, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            List<FileSectionNode> metaDatas = await _metaDataRepo.GetByFileIdAsync(fileId, userId);

            List<ListSectionResponseDTO> responses = new List<ListSectionResponseDTO>();

            foreach (var metaData in metaDatas)
            {
                ListSectionModel data = await _dataRepo.GetByStructureIdAsync(metaData.id);
                responses.Add(new ListSectionResponseDTO(metaData, data));
            }

            return responses;
        }

        /// <summary>
        /// Create File section
        /// </summary>
        /// <param name="newMetaData"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<FileSectionResponseDTO> CreateSectionAsync(FileSectionCreateRequestDTO newMetaData, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            FileSectionNode metaData = await _metaDataRepo.CreateAsync(newMetaData.ToModel(), newMetaData.StructureId, userId);
            ListSectionModel data = new ListSectionModel() { structureId = metaData.id };
            await _dataRepo.CreateAsync(data);

            await _fileService.AddSectionOrderItemAsync(metaData.id, userId, newMetaData.SectionOrderId);

            return new ListSectionResponseDTO(metaData, data);
        }

        /// <summary>
        /// Update File section data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task UpdateDataAsync(ListSectionUpdateRequestDTO data, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);
            FileSectionNode file = await _metaDataRepo.GetByIdAsync(data.Id, userId);

            await UpdateTagsAsync(data.Id, data.Tags, userId);

            await _dataRepo.UpdateAsync(data.Id, data.Items);
        }
    }
}
