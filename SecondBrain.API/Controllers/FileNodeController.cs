using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondBrain.Models.DTOs.FileNode;
using SecondBrain.Services;

namespace SecondBrain.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class FileNodeController : ControllerBase
    {
        private readonly FileNodeService _fileNodeService;

        public FileNodeController(FileNodeService fileNodeService)
        {
            _fileNodeService = fileNodeService;
        }

        /// <summary>
        /// Get all file nodes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetAllAsync()
        {
            try
            {
                List<FileNodeResponseDTO> result = await _fileNodeService.GetAllAsync(User);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Get file by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("id")]
        [Authorize]
        public async Task<ActionResult> GetByIdAsync([FromQuery] Guid id)
        {
            try
            {
                FileNodeResponseDTO result = await _fileNodeService.GetByIdAsync(id, User);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Get file by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("name")]
        [Authorize]
        public async Task<ActionResult> GetByNameAsync([FromQuery] string name)
        {
            try
            {
                List<FileNodeResponseDTO> result = await _fileNodeService.GetByNameAsync(name, User);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Get file by path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("path")]
        [Authorize]
        public async Task<ActionResult> GetByPathAsync([FromQuery] string path)
        {
            try
            {
                List<FileNodeResponseDTO> result = await _fileNodeService.GetByPathAsync(path, User);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Update file data
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPatch]
        [Authorize]
        public async Task<ActionResult> UpdateAsync([FromBody] FileNodeUpdateRequestDTO file)
        {
            try
            {
                await _fileNodeService.UpdateAsync(file, User);
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Create file data
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        public async Task<ActionResult> CreateAsync([FromBody] FileNodeCreateRequestDTO file)
        {
            try
            {
                await _fileNodeService.CreateAsync(file, User);
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteAsync([FromQuery] Guid id)
        {
            try
            {
                await _fileNodeService.DeleteAsync(id, User);
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
