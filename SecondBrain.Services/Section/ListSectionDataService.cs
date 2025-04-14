using SecondBrain.Core;
using SecondBrain.Database.MongoDB;
using SecondBrain.Database.Neo4j;
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

        private readonly string SUB_LOCAL_KEY;


        public ListSectionDataService(FileService fileService, Neo4jGraph graph, FileSectionContext fileSectionContext) : base (graph)
        {
            _metaDataRepo = new FileSectionRepository(graph);
            _dataRepo = new ListSectionRepository(fileSectionContext);
            _dataRepo.CreateIndexAsync().Wait();

            _fileService = fileService;

            SUB_LOCAL_KEY = LOCAL_KEY + "." + Constants.ERROR_SUB_SECTION_LIST_SECTION_DATA;
        }

        /// <summary>
        /// Get File section by id
        /// </summary>
        /// <param name="sectionId"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<ListSectionResponseDTO> GetSectionByIdAsync(Guid sectionId, ClaimsPrincipal claims)
        {
            try
            {
                Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

                FileSectionNode metaData = await _metaDataRepo.GetByIdAsync(sectionId, userId);
                ListSectionModel data = await _dataRepo.GetByStructureIdAsync(metaData.id);

                return new ListSectionResponseDTO(metaData, data);
            }
            catch (Exception ex)
            {
                throw LogHelper.LogError(ex, SUB_LOCAL_KEY, Constants.ERROR_TYPE_LOADING_FAILED);
            }
        }

        /// <summary>
        /// Get File section by file id
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<List<ListSectionResponseDTO>> GetBySectionFileIdAsync(Guid fileId, ClaimsPrincipal claims)
        {
            try
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
            catch (Exception ex)
            {
                throw LogHelper.LogError(ex, SUB_LOCAL_KEY, Constants.ERROR_TYPE_LOADING_FAILED);
            }
        }

        /// <summary>
        /// Create File section
        /// </summary>
        /// <param name="newMetaData"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<FileSectionResponseDTO> CreateSectionAsync(FileSectionCreateRequestDTO newMetaData, ClaimsPrincipal claims)
        {
            try
            {
                Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

                FileSectionNode metaData = await _metaDataRepo.CreateAsync(newMetaData.ToModel(), newMetaData.StructureId, userId);
                ListSectionModel data = new ListSectionModel() { structureId = metaData.id };
                await _dataRepo.CreateAsync(data);

                await _fileService.AddSectionOrderItemAsync(metaData.id, userId, newMetaData.SectionOrderId);

                return new ListSectionResponseDTO(metaData, data);
            }
            catch (Exception ex)
            {
                throw LogHelper.LogError(ex, SUB_LOCAL_KEY, Constants.ERROR_TYPE_CREATE_FAILED);
            }
        }

        /// <summary>
        /// Update File section data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task UpdateDataAsync(ListSectionUpdateRequestDTO data, ClaimsPrincipal claims)
        {
            try
            {
                Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);
                FileSectionNode file = await _metaDataRepo.GetByIdAsync(data.Id, userId);

                await UpdateTagsAsync(data.Id, data.Tags, userId);

                await _dataRepo.UpdateAsync(data.Id, data.Items);
            }
            catch (Exception ex)
            {
                throw LogHelper.LogError(ex, SUB_LOCAL_KEY, Constants.ERROR_TYPE_UPDATE_FAILED);
            }
        }
    }
}
