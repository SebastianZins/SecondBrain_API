using SecondBrain.Models.DatabaseModels.Neo4j;
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
        private readonly FileSectionRepository _fileSectionRepository;
        private readonly ListSectionRepository _listSectionRepository;
        private readonly FileStructureService _fileStructureService;

        public FileService(FileRepository fileRepository, FileSectionRepository fileSectionRepository, ListSectionRepository listSectionRepository, FileStructureService fileStructureService)
        {
            _fileRepository = fileRepository;
            _fileSectionRepository = fileSectionRepository;
            _listSectionRepository = listSectionRepository;
            _fileStructureService = fileStructureService;

            _listSectionRepository.CreateIndexAsync().Wait();
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
        public async Task<List<FileStructureGetResponseDTO>> CreateAsync(FileCreateRequestDTO requestData, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            FileNode file = requestData.ToModel();
            file.id = Guid.NewGuid();
            file.treeId = await _fileStructureService.GetSiblingCountAsync(file.id, userId);
            file = await _fileRepository.CreateAsync(file, (Guid)requestData.ParentFolder!, userId); 
            return await _fileStructureService.GetFileStructureItemsAsync(claims);
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
        /// Delete file and all attached sections
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<List<FileStructureGetResponseDTO>> DeleteAsync(Guid fileId, ClaimsPrincipal claims)
        {
            return await _fileStructureService.DeleteFileStructureItemAsync(fileId, claims);
        }
    }
}
