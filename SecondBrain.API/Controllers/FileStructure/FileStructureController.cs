using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondBrain.Models.DTOs.FileStructure;
using SecondBrain.Services.FileStructure;

namespace SecondBrain.API.Controllers.File
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
        public async Task<ActionResult> GetFileStructureItemsAsync()
        {
            try
            {
                List<FileStructureGetResponseDTO> result = await _fileStructureService.GetFileStructureItemsAsync(User);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> CreateFileStructureItemAsync([FromBody] FileStructureCreateRequestDTO requestData)
        {
            try
            {
                List<FileStructureGetResponseDTO> result = await _fileStructureService.CreateFileStructureItemAsync(requestData, User);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch]
        [Authorize]
        public async Task<ActionResult> UpdateFileStructureItemAsync([FromBody] FileStructureUpdateRequestDTO requestData)
        {
            try
            {
                List<FileStructureGetResponseDTO> result = await _fileStructureService.UpdateFileStructureItemDataAsync(requestData, User);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("move")]
        public async Task<ActionResult> UpdateFileStructureItemTreeIdAsync([FromBody] FileStructureMoveRequestDTO requestData)
        {
            try
            {
                List<FileStructureGetResponseDTO> result = await _fileStructureService.MoveFileStructureItemAsync(requestData, User);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteFileStructureItemAsync([FromQuery] Guid id)
        {
            try
            {
                List<FileStructureGetResponseDTO> result = await _fileStructureService.DeleteFileStructureItemAsync(id, User);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
