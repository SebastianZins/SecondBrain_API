using SecondBrain.Models.DatabaseModels.Neo4j;
using SecondBrain.Models.DTOs.FileNode;
using SecondBrain.Repositories.Neo4j;
using SecondBrain.Utils;
using System.Security.Claims;

namespace SecondBrain.Services
{
    public class FileNodeService
    {
        private readonly FileNodeRepository _fileNodeRepository;


        public FileNodeService(FileNodeRepository fileNodeRepository)
        {
            _fileNodeRepository = fileNodeRepository;
        }

        /// <summary>
        /// Get all files of user
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<List<FileNodeResponseDTO>> GetAllAsync(ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            List<FileNode> nodes = await _fileNodeRepository.GetAllAsync(userId);

            return nodes.Select(node => new FileNodeResponseDTO(node)).ToList();
        }

        /// <summary>
        /// Get file by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<FileNodeResponseDTO> GetByIdAsync(Guid id, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            FileNode node = await _fileNodeRepository.GetByIdAsync(id, userId);

            return new FileNodeResponseDTO(node);
        }

        /// <summary>
        /// Get files by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<List<FileNodeResponseDTO>> GetByNameAsync(string name, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            List<FileNode> nodes = await _fileNodeRepository.GetByNameAsync(name, userId);

            return nodes.Select(node => new FileNodeResponseDTO(node)).ToList();
        }

        /// <summary>
        /// Get files by path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<List<FileNodeResponseDTO>> GetByPathAsync(string path, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            List<FileNode> nodes = await _fileNodeRepository.GetByPathAsync(path, userId);

            return nodes.Select(node => new FileNodeResponseDTO(node)).ToList();
        }

        /// <summary>
        /// Update File
        /// </summary>
        /// <param name="file"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task UpdateAsync(FileNodeUpdateRequestDTO file, ClaimsPrincipal claims)
        {
            Guid currentUserId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            FileNode node = await _fileNodeRepository.GetByIdAsync(file.Id, currentUserId);
            await _fileNodeRepository.UpdateAsync(file.WriteToModel(node), currentUserId);
        }

        /// <summary>
        /// Create File
        /// </summary>
        /// <param name="file"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task CreateAsync(FileNodeCreateRequestDTO file, ClaimsPrincipal claims)
        {
            Guid currentUserId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            FileNode node = file.ToModel();
            await _fileNodeRepository.CreateAsync(node, currentUserId);
        }

        public async Task DeleteAsync(Guid id, ClaimsPrincipal claims)
        {
            Guid userId = ClaimsPrincipalHelper.GetCurrentUserId(claims);

            await _fileNodeRepository.DeleteAsync(id, userId);
        }
    }
}
