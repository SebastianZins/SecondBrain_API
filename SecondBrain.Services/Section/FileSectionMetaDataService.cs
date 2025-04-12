using SecondBrain.Core.Enums;
using SecondBrain.Models.DatabaseModels.MongoDB.File;
using SecondBrain.Models.DatabaseModels.MongoDB.Section;
using SecondBrain.Models.DatabaseModels.Neo4j;
using SecondBrain.Models.DTOs.FileSection;
using SecondBrain.Models.DTOs.FileSection.ChecklistSection;
using SecondBrain.Models.DTOs.FileSection.ListSelection;
using SecondBrain.Models.DTOs.FileSection.MarkdownSection;
using SecondBrain.Models.DTOs.FileSection.OverviewSection;
using SecondBrain.Models.DTOs.FileSection.TableSection;
using SecondBrain.Models.DTOs.FileSection.TextSection;
using SecondBrain.Repositories.MongoDB;
using SecondBrain.Repositories.Neo4j;
using SecondBrain.Services.FileStructure;
using SecondBrain.Utils;
using System.Security.Claims;

namespace SecondBrain.Services.Section
{
    public class FileSectionMetaDataService
    {
        private readonly FileService _fileService;
        private readonly FileSectionRepository _fileSectionRepository;
        private readonly ListSectionRepository _listSectionRepository;


        public FileSectionMetaDataService(FileService fileService, FileSectionRepository fileSectionRepository, ListSectionRepository listSectionRepository)
        {
            _fileService = fileService;
            _fileSectionRepository = fileSectionRepository;
            _listSectionRepository = listSectionRepository;
        }

        /// <summary>
        /// Get File section meta data by id
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<FileSectionResponseDTO> GetFileSectionMetaDataById(Guid fileId, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            FileSectionNode file = await _fileSectionRepository.GetByIdAsync(fileId, userId);

            return new FileSectionResponseDTO(file);
        }

        /// <summary>
        /// Get File section meta data by file id
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<List<FileSectionResponseDTO>> GetFileSectionMetaDataByFileId(Guid fileId, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            List<FileSectionNode> files = await _fileSectionRepository.GetByFileIdAsync(fileId, userId);

            return files.Select(file => new FileSectionResponseDTO(file)).ToList();
        }

        /// <summary>
        /// Update File section meta data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task UpdateFileSectionMetaDataAsync(FileSectionUpdateRequestDTO data, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);
            FileSectionNode file = await _fileSectionRepository.GetByIdAsync(data.Id, userId);

            await _fileSectionRepository.UpdateAsync(data.WriteToModel(file), userId);
        }

        /// <summary>
        /// Delete File section
        /// </summary>
        /// <param name="id"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task DeleteFileSectionMetaDataAsync(Guid id, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);
            FileSectionNode section = await _fileSectionRepository.GetByIdAsync(id, userId);

            // delete data
            switch (section.sectionType)
            {
                case ESectionType.TEXT:
                    break;
                case ESectionType.MARKDOWN:
                    break;
                case ESectionType.LIST:
                    await _listSectionRepository.DeleteAsync(id);
                    break;
                case ESectionType.CHECK_LIST:
                    break;
                case ESectionType.TABLE:
                    break;
                case ESectionType.OVERVIEW:
                    break;
            }

            // update file section order list
            FileNode file = await _fileService.GetBySectionIdAsync(id, userId);
            file.sectionsOrder.Remove(id);
            await _fileService.UpdateAsync(file, userId);

            // delete meta data
            await _fileSectionRepository.DeleteAsync(id, userId);
        }

        /// <summary>
        /// Create File section
        /// </summary>
        /// <param name="newMetaData"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<FileSectionResponseDTO?> CreateSectionAsync(FileSectionCreateRequestDTO newMetaData, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            FileSectionNode metaData = await _fileSectionRepository.CreateAsync(newMetaData.ToModel(), newMetaData.StructureId, userId);
            FileSectionResponseDTO? response = null;

            switch (newMetaData.SectionType)
            {
                case ESectionType.TEXT:
                    TextSectionModel textData = new TextSectionModel() { structureId = metaData.id };
                    response = new TextSectionResponseDTO(metaData, textData);
                    break;
                case ESectionType.MARKDOWN:
                    MarkdownSectionModel markdownData = new MarkdownSectionModel() { structureId = metaData.id };
                    response = new MarkdownSectionResponseDTO(metaData, markdownData);
                    break;
                case ESectionType.LIST:
                    ListSectionModel listData = new ListSectionModel() { structureId = metaData.id };
                    await _listSectionRepository.CreateAsync(listData);
                    response = new ListSectionResponseDTO(metaData, listData);
                    break;
                case ESectionType.CHECK_LIST:
                    ChecklistSectionModel checklistData = new ChecklistSectionModel() { structureId = metaData.id };
                    response = new ChecklistSectionResponseDTO(metaData, checklistData);
                    break;
                case ESectionType.TABLE:
                    TableSectionModel tableData = new TableSectionModel() { structureId = metaData.id };
                    response = new TableSectionResponseDTO(metaData, tableData);
                    break;
                case ESectionType.OVERVIEW:
                    OverviewSectionModel overviewData = new OverviewSectionModel() { structureId = metaData.id };
                    response = new OverviewSectionResponseDTO(metaData, overviewData);
                    break;
            }

            await _fileService.AddSectionOrderItemAsync(metaData.id, userId, newMetaData.SectionOrderId);

            return response;
        }
    }
}
