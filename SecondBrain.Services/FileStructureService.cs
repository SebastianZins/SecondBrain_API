using SecondBrain.Models.DatabaseModels.Neo4j;
using SecondBrain.Models.DTOs.FileStructure;
using SecondBrain.Repositories.Neo4j;
using SecondBrain.Utils;
using System.Security.Claims;

namespace SecondBrain.Services
{
    public class FileStructureService
    {
        private readonly FileStructureRepository _fileStructureRepository;


        public FileStructureService(FileStructureRepository fileStructureRepository)
        {
            _fileStructureRepository = fileStructureRepository;
        }

        /// <summary>
        /// Get complete file structure sorted in tree view starting from root folder
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<List<FileStructureGetResponseDTO>> GetFileStructureAsync(ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            FileStructureNode? root = await _fileStructureRepository.GetRootFolderAsync(userId);

            // create root folder if it doesn't exist
            if (root == null)
            {
                FileStructureNode emptyFolderData = new FileStructureNode()
                {
                    id = Guid.NewGuid(),
                    name = "root",
                };

                root = await _fileStructureRepository.CreateRootFolderAsync(emptyFolderData, userId);
            }

            List<Tuple<FileStructureNode, FileStructureNode>> folderConnections = await _fileStructureRepository.GetFileStructureAsync(userId);

            FileStructureGetResponseDTO response = _SortStructure(root, folderConnections);

            if (response.Children != null)
            {
                return response.Children;
            } else
            {
                return new List<FileStructureGetResponseDTO>();
            }
        }

        /// <summary>
        /// Create new folder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<List<FileStructureGetResponseDTO>> CreateFolderAsync(FileStructureCreateRequestDTO folder, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            await _fileStructureRepository.CreateFolderAsync(folder.ToModel(), folder?.ParentFolder, userId);

            return await GetFileStructureAsync(claims);
        }

        /// <summary>
        /// Update folder data
        /// </summary>
        /// <param name="folderData"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<List<FileStructureGetResponseDTO>> UpdateFolderAsync(FileStructureUpdateRequestDTO folderData, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            FileStructureNode folder = await _fileStructureRepository.GetFolderAsync(folderData.Id, userId);

            await _fileStructureRepository.UpdateFolderAsync(folderData.WriteToModel(folder), userId);

            return await GetFileStructureAsync(claims);
        }

        /// <summary>
        /// Move folder to other parent folder
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="newParentFolderId"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<List<FileStructureGetResponseDTO>> MoveFolderAsync(Guid folderId, Guid? newParentFolderId, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            if (newParentFolderId == null)
            {
                newParentFolderId = (await _fileStructureRepository.GetRootFolderAsync(userId))!.id;
            }

            await _fileStructureRepository.MoveFolderAsync(folderId, (Guid)newParentFolderId, userId);

            return await GetFileStructureAsync(claims);
        }

        /// <summary>
        /// Recursive step to sort file structure to tree view
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="folderConnections"></param>
        /// <returns></returns>
        private FileStructureGetResponseDTO _SortStructure(FileStructureNode folder, List<Tuple<FileStructureNode, FileStructureNode>> folderConnections)
        {
            FileStructureGetResponseDTO response = new FileStructureGetResponseDTO(folder, Core.Enums.EFileType.FOLDER);
            List<Tuple<FileStructureNode, FileStructureNode>> filteredFolderConnections = folderConnections
                .Where(f => f.Item1.id == folder.id)
                .ToList();

            if (folderConnections.Count > 0)
            {
                response.Children = new List<FileStructureGetResponseDTO>();
                foreach (var connection in filteredFolderConnections)
                {
                    FileStructureGetResponseDTO itemResponse = _SortStructure(connection.Item1, folderConnections);
                    response.Children.Add(itemResponse);
                }
            }

            return response;
        }
    }
}
