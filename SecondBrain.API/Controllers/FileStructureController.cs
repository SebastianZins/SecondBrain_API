using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondBrain.Models.DTOs.FileStructure;
using SecondBrain.Services;

namespace SecondBrain.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class FileStructureController : ControllerBase
    {
        private readonly FileStructureService _fileStructureService;

        public FileStructureController(FileStructureService fileStructureService)
        {
            _fileStructureService = fileStructureService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetFileStructureAsync()
        {
            try
            {
                List<FileStructureGetResponseDTO> result = await _fileStructureService.GetFileStructureAsync(User);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> CreateFolderAsync([FromBody] FileStructureCreateRequestDTO requestData)
        {
            try
            {
                List<FileStructureGetResponseDTO> result = await _fileStructureService.CreateFolderAsync(requestData, User);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch]
        [Authorize]
        public async Task<ActionResult> UpdateFolderAsync([FromBody] FileStructureUpdateRequestDTO requestData)
        {
            try
            {
                List<FileStructureGetResponseDTO> result = await _fileStructureService.UpdateFolderAsync(requestData, User);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch]
        [Route("move")]
        [Authorize]
        public async Task<ActionResult> MoveFolderAsync([FromQuery] Guid folderId, Guid? newParentFolderId)
        {
            try
            {
                List<FileStructureGetResponseDTO> result = await _fileStructureService.MoveFolderAsync(folderId, newParentFolderId, User);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
