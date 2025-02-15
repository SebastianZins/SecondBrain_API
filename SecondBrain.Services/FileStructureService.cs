using SecondBrain.Core.Enums;
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
        public async Task<List<FileStructureGetResponseDTO>> GetFileStructureItemsAsync(ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            FileStructureNode root = await _GetFileStructureItem(userId, null, true);

            List<FileStructureNode> fileStructures = await _fileStructureRepository.GetFileStructuresAsync(userId);
            List<Tuple<Guid, Guid>> connections = await _fileStructureRepository.GetFileStructureConnectionsAsync(userId);

            return _buildTree(root.id, fileStructures, connections)?.Children ?? new List<FileStructureGetResponseDTO>();
        }

        /// <summary>
        /// Create new file structure item
        /// </summary>
        /// <param name="requestData"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<List<FileStructureGetResponseDTO>> CreateFileStructureItemAsync(FileStructureCreateRequestDTO requestData, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);
            FileStructureNode parent = await _GetFileStructureItem(userId, requestData.ParentFolder);

            List<FileStructureNode> siblings = await _fileStructureRepository.GetChildFileStructureItemsAsync(parent.id, userId);
            await _fileStructureRepository.CreateFileStructureItemAsync(requestData.Label, requestData.Type, siblings.Count, parent.id, userId);
            return await GetFileStructureItemsAsync(claims);
        }

        /// <summary>
        /// Update folder data
        /// </summary>
        /// <param name="requestData"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<List<FileStructureGetResponseDTO>> UpdateFileStructureItemDataAsync(FileStructureUpdateRequestDTO requestData, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);
            await _GetFileStructureItem(userId, requestData.Id);

            await _fileStructureRepository.UpdateFileStructureItemDataAsync(requestData.Id, requestData.Label, requestData.Type, userId);
            return await GetFileStructureItemsAsync(claims);
        }

        /// <summary>
        /// Move file structure item either in same folder or to other
        /// Rearange new and old sibling items
        /// </summary>
        /// <param name="requestData"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<List<FileStructureGetResponseDTO>> MoveFileStructureItemAsync(FileStructureMoveRequestDTO requestData, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);
            FileStructureNode? root = await _fileStructureRepository.GetFileStructureItemAsync(userId);
            FileStructureNode currentParent = await _fileStructureRepository.GetParentFolderAsync(requestData.Id, userId);
            List<FileStructureNode> currentSiblings = await _fileStructureRepository.GetChildFileStructureItemsAsync(currentParent.id, userId);

            // moved in the same folder
            if (requestData.ParentId == currentParent.id || (requestData.ParentId == null && root?.id == currentParent.id))
            {
                await ShiftFileStructureItemLeft(requestData.OldTreeId, requestData.NewTreeId, userId, currentParent, currentSiblings);
                await ShiftFileStructureItemRight(requestData.OldTreeId, requestData.NewTreeId, userId, currentParent, currentSiblings);
                await _fileStructureRepository.UpdateItemTreeIdAsync(requestData.Id, requestData.NewTreeId, userId);
            }
            // moved to other folder
            else
            {
                await ShiftFileStructureItemLeft(requestData.OldTreeId, int.MaxValue, userId, currentParent, currentSiblings);
                await ShiftFileStructureItemRight(requestData.OldTreeId, int.MaxValue, userId, currentParent, currentSiblings);

                FileStructureNode newParent = await _GetFileStructureItem(userId, requestData.ParentId);
                List<FileStructureNode> newSiblings = await _fileStructureRepository.GetChildFileStructureItemsAsync(newParent.id, userId);
                await ShiftFileStructureItemLeft(int.MaxValue, requestData.NewTreeId, userId, newParent, newSiblings);
                await ShiftFileStructureItemRight(int.MaxValue, requestData.NewTreeId, userId, newParent, newSiblings);
                await _fileStructureRepository.UpdateItemTreeIdAsync(requestData.Id, requestData.NewTreeId, userId);
                await _fileStructureRepository.MoveFielStructureItemAsync(requestData.Id, newParent.id, userId);
            }
            return await GetFileStructureItemsAsync(claims);
        }

        /// <summary>
        /// Delete file structure item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<List<FileStructureGetResponseDTO>> DeleteFileStructureItemAsync(Guid id, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            FileStructureNode item = await _GetFileStructureItem(userId, id);
            FileStructureNode parent = await _fileStructureRepository.GetParentFolderAsync(id, userId);
            List<FileStructureNode> siblings = await _fileStructureRepository.GetChildFileStructureItemsAsync(parent.id, userId);
            await ShiftFileStructureItemLeft(item.treeId, int.MaxValue, userId, parent, siblings);
            await _fileStructureRepository.DeleteFileStructureItemAsync(id, userId);

            return await GetFileStructureItemsAsync(claims);
        }

        /// <summary>
        /// Build a tree from file structure items and connections
        /// </summary>
        /// <param name="rootId"></param>
        /// <param name="fileStructures"></param>
        /// <param name="connections"></param>
        /// <returns></returns>
        private FileStructureGetResponseDTO _buildTree(Guid rootId, List<FileStructureNode> fileStructures, List<Tuple<Guid, Guid>> connections)
        {
            Dictionary<Guid, FileStructureGetResponseDTO> responses = fileStructures
                .Select(f => new KeyValuePair<Guid, FileStructureGetResponseDTO>(f.id, new FileStructureGetResponseDTO(f)))
                .ToDictionary();

            foreach (var connection in connections)
            {
                var parent = responses[connection.Item1];
                var child = responses[connection.Item2];

                if (parent.Children == null)
                {
                    parent.Children = new List<FileStructureGetResponseDTO>([child]);
                } 
                else
                {
                    parent.Children.Add(child);
                }
            }

            foreach (var response in responses)
            {
                if (response.Value.Children != null)
                {
                    response.Value.Children = response.Value.Children.OrderBy(r => r.TreeId).ToList();
                }
            }

            return responses[rootId];
        }

        /// <summary>
        /// Shift tree id of siblings to the left
        /// </summary>
        /// <param name="oldTreeId"></param>
        /// <param name="newTreeId"></param>
        /// <param name="userId"></param>
        /// <param name="parent"></param>
        /// <param name="siblings"></param>
        /// <returns></returns>
        private async Task ShiftFileStructureItemLeft(int oldTreeId, int newTreeId, Guid userId, FileStructureNode parent, List<FileStructureNode> siblings)
        {
            foreach (var sibling in siblings.Where(s => s.treeId > oldTreeId && s.treeId <= newTreeId))
            {
                await _fileStructureRepository.UpdateItemTreeIdAsync(sibling.id, sibling.treeId - 1, userId);
            }
        }

        /// <summary>
        /// Shift tree id of siblings to the right
        /// </summary>
        /// <param name="oldTreeId"></param>
        /// <param name="newTreeId"></param>
        /// <param name="userId"></param>
        /// <param name="parent"></param>
        /// <param name="siblings"></param>
        /// <returns></returns>
        private async Task ShiftFileStructureItemRight(int oldTreeId, int newTreeId, Guid userId, FileStructureNode parent, List<FileStructureNode> siblings)
        {
            foreach (var sibling in siblings.Where(s => s.treeId < oldTreeId && s.treeId >= newTreeId))
            {
                await _fileStructureRepository.UpdateItemTreeIdAsync(sibling.id, sibling.treeId + 1, userId);
            }
        }

        /// <summary>
        /// Load a file structure item
        /// if it doesnt exist throw error or create a root folder if flag is set
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="itemId"></param>
        /// <param name="createRootIfNotFound"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<FileStructureNode> _GetFileStructureItem(Guid userId, Guid? itemId = null, bool createRootIfNotFound = false)
        {
            FileStructureNode? item = await _fileStructureRepository.GetFileStructureItemAsync(userId, itemId);
            if (item == null)
            {
                if (createRootIfNotFound)
                {
                    item = await _fileStructureRepository.CreateFileStructureRootAsync("root", EFileType.FOLDER, 0, userId);
                }
                else
                {
                    throw new Exception("Error: File structure item not found.");
                }
            }
            return item;
        }
    }
}
