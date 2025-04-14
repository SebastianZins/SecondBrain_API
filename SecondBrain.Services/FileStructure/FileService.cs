using SecondBrain.Core.Enums;
using SecondBrain.Database.MongoDB;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.DatabaseModels.MongoDB.File;
using SecondBrain.Models.DatabaseModels.MongoDB.Section;
using SecondBrain.Models.DatabaseModels.Neo4j;
using SecondBrain.Models.DTOs.FileSection.ChecklistSection;
using SecondBrain.Models.DTOs.FileSection.ListSelection;
using SecondBrain.Models.DTOs.FileSection.TextSection;
using SecondBrain.Models.DTOs.FileStructure;
using SecondBrain.Models.DTOs.FileStructure.File;
using SecondBrain.Repositories.MongoDB;
using SecondBrain.Repositories.Neo4j;
using SecondBrain.Utils;
using System.Security.Claims;

namespace SecondBrain.Services.FileStructure
{
    public class FileService
    {
        private readonly FileRepository _fileRepository;
        private readonly ListSectionRepository _listSectionRepository;
        private readonly ChecklistSectionRepository _checklistSectionRepository;
        private readonly TextSectionRepository _textSectionRepository;
        private readonly FileStructureService _fileStructureService;
        private readonly FileSectionRepository _fileSectionRepository;

        public FileService(FileStructureService fileStructureService, Neo4jGraph graph, FileSectionContext fileSectionContext)
        {
            _fileRepository = new FileRepository(graph);
            _listSectionRepository = new ListSectionRepository(fileSectionContext);
            _checklistSectionRepository = new ChecklistSectionRepository(fileSectionContext);
            _textSectionRepository = new TextSectionRepository(fileSectionContext);
            _fileSectionRepository = new FileSectionRepository(graph);
            _fileStructureService = fileStructureService;

            _listSectionRepository.CreateIndexAsync().Wait();
            _checklistSectionRepository.CreateIndexAsync().Wait();
            _textSectionRepository.CreateIndexAsync().Wait();
        }

        /// <summary>
        /// Get File info by id
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<FileResponseDTO> GetByIdAsync(Guid fileId, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            FileNode file = await _fileRepository.GetByIdAsync(fileId, userId);
            return new FileResponseDTO(file);
        }

        /// <summary>
        /// Get File info by id
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<FileDataResponseDTO> GetByIdWithDataAsync(Guid fileId, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            FileNode file = await _fileRepository.GetByIdAsync(fileId, userId);
            FileDataResponseDTO response = new FileDataResponseDTO(file);

            List<FileSectionNode> sections = await _fileSectionRepository.GetByFileIdAsync(file.id, userId);

            //// text sections
            List<FileSectionNode> textSectionMetaData = sections.Where(s => s.sectionType == ESectionType.TEXT).ToList();
            List<TextSectionModel> textSectionModels = await _textSectionRepository.GetByStructureIdsAsync(textSectionMetaData.Select(s => s.id).ToList());
            response.TextSections = textSectionModels.Select((model, i) => new TextSectionResponseDTO(textSectionMetaData[i], model)).ToList();
            //// markdown sections
            //List<FileSectionNode> markdownSectionMetaData = sections.Where(s => s.sectionType == ESectionType.MARKDOWN).ToList();
            //List<MarkdownSectionModel> markdownSectionModels = await _markdownSectionRepository.GetByStructureIdsAsync(markdownSectionMetaData.Select(s => s.id).ToList());
            //response.MarkdownSections = markdownSectionModels.Select((model, i) => new MarkdownSectionResponseDTO(markdownSectionMetaData[i], model)).ToList();
            // list sections
            List<FileSectionNode> listSectionMetaData = sections.Where(s => s.sectionType == ESectionType.LIST).ToList();
            List<ListSectionModel> listSectionModels = await _listSectionRepository.GetByStructureIdsAsync(listSectionMetaData.Select(s => s.id).ToList());
            response.ListSections = listSectionModels.Select((model, i) => new ListSectionResponseDTO(listSectionMetaData[i], model)).ToList();
            //// checkList sections
            List<FileSectionNode> checkListSectionMetaData = sections.Where(s => s.sectionType == ESectionType.CHECK_LIST).ToList();
            List<ChecklistSectionModel> checkListSectionModels = await _checklistSectionRepository.GetByStructureIdsAsync(checkListSectionMetaData.Select(s => s.id).ToList());
            response.CheckListSections = checkListSectionModels.Select((model, i) => new ChecklistSectionResponseDTO(checkListSectionMetaData[i], model)).ToList();
            //// table sections
            //List<FileSectionNode> tableSectionMetaData = sections.Where(s => s.sectionType == ESectionType.TABLE).ToList();
            //List<TableSectionModel> tableSectionModels = await _tableSectionRepository.GetByStructureIdsAsync(tableSectionMetaData.Select(s => s.id).ToList());
            //response.TableSections = tableSectionModels.Select((model, i) => new TableSectionResponseDTO(tableSectionMetaData[i], model)).ToList();
            //// overview sections
            //List<FileSectionNode> overviewSectionMetaData = sections.Where(s => s.sectionType == ESectionType.CHECK_LIST).ToList();
            //List<OverviewSectionModel> overviewSectionModels = await _overviewSectionRepository.GetByStructureIdsAsync(overviewSectionMetaData.Select(s => s.id).ToList());
            //response.OverviewSections = overviewSectionModels.Select((model, i) => new OverviewSectionResponseDTO(overviewSectionMetaData[i], model)).ToList();
            return response;
        }

        /// <summary>
        /// Get File info by file structure item (so by path)
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<FileResponseDTO> GetByFileStructureItemAsync(Guid folderId, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            FileNode file = await _fileRepository.GetByFileStructureItemAsync(folderId, userId);
            return new FileResponseDTO(file);
        }

        /// <summary>
        /// Create File
        /// </summary>
        /// <param name="requestData"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<FileResponseDTO> CreateAsync(FileCreateRequestDTO requestData, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            FileNode file = requestData.ToModel();
            file.id = Guid.NewGuid();
            FileStructureNode parent = await _fileStructureService.GetFileStructureItem(userId, requestData.ParentFolder, true);
            file.treeId = (await _fileStructureService.GetChildCountAsync(parent.id, userId));
            file = await _fileRepository.CreateAsync(file, parent.id, userId);
            return new FileResponseDTO(file);
        }

        /// <summary>
        /// Update File data
        /// </summary>
        /// <param name="requestData"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<FileResponseDTO> UpdateAsync(FileUpdateRequestDTO requestData, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            FileNode file = await _fileRepository.GetByIdAsync(requestData.Id, userId);

            file = await _fileRepository.UpdateAsync(requestData.WriteToModel(file), userId);
            return new FileResponseDTO(file);
        }

        /// <summary>
        /// Update File data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<FileResponseDTO> UpdateAsync(FileNode data, Guid userId)
        {
            FileNode file = await _fileRepository.UpdateAsync(data, userId);
            return new FileResponseDTO(file);
        }

        /// <summary>
        /// Delete file and all attached sections
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<List<FileStructureGetResponseDTO>> DeleteAsync(Guid fileId, ClaimsPrincipal claims)
        {
            await _fileStructureService.DeleteFileStructureItemAsync(fileId, claims);

            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);
            await _fileRepository.DeleteAsync(fileId, userId);

            return await _fileStructureService.GetFileStructureItemsAsync(claims);
        }

        public async Task<FileNode> GetBySectionIdAsync(Guid sectionId, Guid userId)
        {
            return await _fileRepository.GetBySectionIdAsync(sectionId, userId);
        }

        public async Task AddSectionOrderItemAsync(Guid sectionId, Guid userId, int position)
        {
            FileNode file = await _fileRepository.GetBySectionIdAsync(sectionId, userId);
            file.sectionsOrder.Insert(position, sectionId);

            await _fileRepository.UpdateAsync(file, userId);
        }

        public async Task UpdateSectionOrderAsync(List<Guid> sectionOrder, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            FileNode file = await _fileRepository.GetBySectionIdAsync(sectionOrder[0], userId);
            file.sectionsOrder = sectionOrder;

            await _fileRepository.UpdateAsync(file, userId);
        }
    }
}
