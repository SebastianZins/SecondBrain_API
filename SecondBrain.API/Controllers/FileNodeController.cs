using Microsoft.AspNetCore.Mvc;
using SecondBrain.Models.DTOs;
using SecondBrain.Models.DTOs.FileNode;
using SecondBrain.Services;

namespace SecondBrain.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileNodeController : ControllerBase
    {
        private readonly FileNodeService _fileNodeService;

        public FileNodeController(FileNodeService fileNodeService)
        {
            _fileNodeService = fileNodeService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAsync()
        {
            try
            {
                List<FileNodeResponseDTO> result = await _fileNodeService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("id")]
        public async Task<ActionResult> GetByIdAsync([FromQuery] Guid id)
        {
            try
            {
                FileNodeResponseDTO result = await _fileNodeService.GetByIdAsync(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("name")]
        public async Task<ActionResult> GetByNameAsync([FromQuery] string name)
        {
            try
            {
                List<FileNodeResponseDTO> result = await _fileNodeService.GetByNameAsync(name);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("path")]
        public async Task<ActionResult> GetByPathAsync([FromQuery] string path)
        {
            try
            {
                List<FileNodeResponseDTO> result = await _fileNodeService.GetByPathAsync(path);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch]
        public async Task<ActionResult> UpsertAsync([FromBody] FileNodeResponseDTO file)
        {
            try
            {
                await _fileNodeService.UpsertAsync(file);
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAsync([FromQuery] Guid id)
        {
            try
            {
                await _fileNodeService.DeleteAsync(id);
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
