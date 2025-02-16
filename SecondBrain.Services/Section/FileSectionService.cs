using SecondBrain.Models.DatabaseModels.Neo4j;
using SecondBrain.Models.DTOs.FileSection;
using SecondBrain.Repositories.Neo4j;
using SecondBrain.Utils;
using System.Security.Claims;

namespace SecondBrain.Services.Section
{
    public class FileSectionService
    {
        private readonly FileSectionRepository _fileSectionRepository;


        public FileSectionService(FileSectionRepository fileSectionRepository)
        {
            _fileSectionRepository = fileSectionRepository;
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
    }
}
