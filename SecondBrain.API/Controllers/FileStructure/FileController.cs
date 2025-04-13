using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondBrain.Database.MongoDB;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.DTOs.FileStructure.File;
using SecondBrain.Services.FileStructure;

namespace SecondBrain.API.Controllers.File
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly FileStructureService _fileStructureService;
        private readonly FileService _fileService;

        public FileController(FileService fileService, FileStructureService fileStructureService)
        {
            _fileService = fileService;
            _fileStructureService = fileStructureService;
        }

        /// <summary>
        /// Get File info by id
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetByIdWithDataAsync([FromQuery] Guid fileId)
        {
            try
            {
                return Ok(await _fileService.GetByIdWithDataAsync(fileId, User));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Get File info by file structure item (so by path)
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        [HttpGet("byFolder")]
        [Authorize]
        public async Task<ActionResult> GetByFileStructureItemAsync([FromQuery] Guid folderId)
        {
            try
            {
                return Ok(await _fileService.GetByFileStructureItemAsync(folderId, User));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Create File
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        public async Task<ActionResult> CreateAsync([FromBody] FileCreateRequestDTO requestData)
        {
            try
            {                
                return Ok(await _fileService.CreateAsync(requestData, User));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Update File data
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [HttpPatch]
        [Authorize]
        public async Task<ActionResult> UpdateAsync([FromBody] FileUpdateRequestDTO requestData)
        {
            try
            {
                await _fileService.UpdateAsync(requestData, User);
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Update File section order
        /// </summary>
        /// <param name="fileSectionOrder"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("order")]
        [Authorize]
        public async Task<ActionResult> UpdateAsync([FromBody] List<Guid> fileSectionOrder)
        {
            try
            {
                await _fileService.UpdateSectionOrderAsync(fileSectionOrder, User);
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Delete file and all attached sections
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteAsync([FromQuery] Guid fileId)
        {
            try
            {
                await _fileService.DeleteAsync(fileId, User);
                return Ok(await _fileStructureService.GetFileStructureItemsAsync(User));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
