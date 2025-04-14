using SecondBrain.Core;
using SecondBrain.Database.MongoDB;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.DatabaseModels.MongoDB.File;
using SecondBrain.Models.DatabaseModels.MongoDB.Section;
using SecondBrain.Models.DatabaseModels.Neo4j;
using SecondBrain.Models.DTOs.FileSection;
using SecondBrain.Models.DTOs.FileSection.ChecklistSection;
using SecondBrain.Repositories.MongoDB;
using SecondBrain.Repositories.Neo4j;
using SecondBrain.Services.FileStructure;
using SecondBrain.Utils;
using System.Security.Claims;

namespace SecondBrain.Services.Section
{
    public class ChecklistSectionDataService : FileSectionDataService
    {
        private readonly ChecklistSectionRepository _dataRepo;
        private readonly FileSectionRepository _metaDataRepo;
        private readonly FileService _fileService;

        private readonly string SUB_LOCAL_KEY;

        public ChecklistSectionDataService(FileService fileService, Neo4jGraph graph, FileSectionContext fileSectionContext) : base(graph)
        {
            _metaDataRepo = new FileSectionRepository(graph);
            _dataRepo = new ChecklistSectionRepository(fileSectionContext);
            _dataRepo.CreateIndexAsync().Wait();

            _fileService = fileService;

            SUB_LOCAL_KEY = LOCAL_KEY + "." + SUB_LOCAL_KEY;
        }

        /// <summary>
        /// Get File section by id
        /// </summary>
        /// <param name="sectionId"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<ChecklistSectionResponseDTO> GetSectionByIdAsync(Guid sectionId, ClaimsPrincipal claims)
        {
            try
            {
                Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

                FileSectionNode metaData = await _metaDataRepo.GetByIdAsync(sectionId, userId);
                ChecklistSectionModel data = await _dataRepo.GetByStructureIdAsync(metaData.id);

                return new ChecklistSectionResponseDTO(metaData, data);
            }
            catch (Exception ex)
            {
                throw LogHelper.LogError(ex, LOCAL_KEY, Constants.ERROR_TYPE_LOADING_FAILED);
            }
        }

        /// <summary>
        /// Get File section by file id
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<List<ChecklistSectionResponseDTO>> GetBySectionFileIdAsync(Guid fileId, ClaimsPrincipal claims)
        {
            try
            {
                Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

                List<FileSectionNode> metaDatas = await _metaDataRepo.GetByFileIdAsync(fileId, userId);

                List<ChecklistSectionResponseDTO> responses = new List<ChecklistSectionResponseDTO>();

                foreach (var metaData in metaDatas)
                {
                    ChecklistSectionModel data = await _dataRepo.GetByStructureIdAsync(metaData.id);
                    responses.Add(new ChecklistSectionResponseDTO(metaData, data));
                }

                return responses;
            }
            catch (Exception ex)
            {
                throw LogHelper.LogError(ex, LOCAL_KEY, Constants.ERROR_TYPE_LOADING_FAILED);
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
                ChecklistSectionModel data = new ChecklistSectionModel() { structureId = metaData.id };
                await _dataRepo.CreateAsync(data);

                await _fileService.AddSectionOrderItemAsync(metaData.id, userId, newMetaData.SectionOrderId);

                return new ChecklistSectionResponseDTO(metaData, data);
            }
            catch (Exception ex)
            {
                throw LogHelper.LogError(ex, LOCAL_KEY, Constants.ERROR_TYPE_CREATE_FAILED);
            }
        }

        /// <summary>
        /// Update File section data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task UpdateDataAsync(ChecklistSectionUpdateRequestDTO data, ClaimsPrincipal claims)
        {
            try
            {
                Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);
                FileSectionNode file = await _metaDataRepo.GetByIdAsync(data.Id, userId);

                await UpdateTagsAsync(data.Id, data.Tags, userId);

                List<ChecklistItemModel> listItems = data.Items.Select(i => i.ToModel()).ToList();
                await _dataRepo.UpdateAsync(data.Id, listItems);
            }
            catch (Exception ex)
            {
                throw LogHelper.LogError(ex, LOCAL_KEY, Constants.ERROR_TYPE_UPDATE_FAILED);
            }
        }
    }
}
